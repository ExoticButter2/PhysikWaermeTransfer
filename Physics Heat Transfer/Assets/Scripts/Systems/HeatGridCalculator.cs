using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial struct HeatGridCalculator : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder().WithAll<HeatData>().Build();
        query.CompleteDependency();

        int heatDataEntityCount = query.CalculateEntityCount();

        var deltaTemperaturesArray = new NativeArray<float>(heatDataEntityCount, Allocator.TempJob, NativeArrayOptions.ClearMemory);

        SystemHandle heatGridGeneratorHandle = state.World.GetExistingSystem<HeatGridGenerator>();
        ref var generatorState = ref state.WorldUnmanaged.GetExistingSystemState<HeatGridGenerator>();

        state.Dependency = JobHandle.CombineDependencies(state.Dependency, generatorState.Dependency);

        HeatGridGenerator heatGridGeneratorSystem = state.WorldUnmanaged.GetUnsafeSystemRef<HeatGridGenerator>(heatGridGeneratorHandle);

        state.Dependency.Complete();

        ComponentLookup<HeatShaderValues> heatShaderValuesFromEntity = SystemAPI.GetComponentLookup<HeatShaderValues>(false);

        NativeParallelHashMap<int4, HeatData> coordinateIDToHeatDataHashmap = heatGridGeneratorSystem.CoordinateIDToHeatDataHashmap;
        NativeParallelHashMap<int, GridData> gridIDToGridDataHashmap = heatGridGeneratorSystem.GridIDToGridDataHashmap;

        coordinateIDToHeatDataHashmap.Clear();

        UpdateHashMapJob updateHashMapJob = new UpdateHashMapJob
        {
            HashMap = coordinateIDToHeatDataHashmap.AsParallelWriter()
        };

        JobHandle updateHashMapJobHandle = updateHashMapJob.ScheduleParallel(state.Dependency);

        CalculateTemperatureJob calculateTemperatureJob = new CalculateTemperatureJob
        {
            DeltaTemperatures = deltaTemperaturesArray,
            DeltaTime = SystemAPI.Time.DeltaTime,
            GridIdToGridDataHashmap = gridIDToGridDataHashmap,
            GridEntryMap = coordinateIDToHeatDataHashmap
        };

        JobHandle calculationHandle = calculateTemperatureJob.ScheduleParallel(updateHashMapJobHandle);
        calculationHandle.Complete();

        ApplyTemperatureJob applyTemperatureJob = new ApplyTemperatureJob
        {
            TemperaturesToApply = deltaTemperaturesArray,
            HeatShaderValuesFromEntity = heatShaderValuesFromEntity
        };

        JobHandle applyHandle = applyTemperatureJob.ScheduleParallel(calculationHandle);

        state.Dependency = applyHandle;

        deltaTemperaturesArray.Dispose(state.Dependency);
    }
}

[BurstCompile]
public partial struct UpdateHashMapJob : IJobEntity
{
    public NativeParallelHashMap<int4, HeatData>.ParallelWriter HashMap;

    public void Execute(in HeatData data)
    {
        HashMap.TryAdd(data.XYZWithGridID, data);
    }
}

[BurstCompile]
public partial struct CalculateTemperatureJob : IJobEntity
{
    [ReadOnly] public NativeParallelHashMap<int4, HeatData> GridEntryMap;
    [ReadOnly] public NativeParallelHashMap<int, GridData> GridIdToGridDataHashmap;
    public float DeltaTime;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> DeltaTemperatures;

    private static readonly int4[] offsets = new int4[]
    {
        new int4(1, 0, 0, 0),
        new int4(0, 1, 0, 0),
        new int4(0, 0, 1, 0),
        new int4(-1, 0, 0, 0),
        new int4(0, -1, 0, 0),
        new int4(0, 0, -1, 0)
    };

    [BurstCompile]
    public void Execute(in HeatData data)
    {
        if (!GridIdToGridDataHashmap.TryGetValue(data.XYZWithGridID.w, out GridData gridData))
            return;

        float currentTemperature = data.temperature;
        float totalDeltaTemperature = 0f;

        int4 currentId = new int4(data.XYZWithGridID.x, data.XYZWithGridID.y, data.XYZWithGridID.z, data.XYZWithGridID.w);

        for (int i = 0; i < offsets.Length; i++)//get all neighbors
        {
            float cellSize = gridData.cellSize;
            int4 neighborCoordinateID = currentId + offsets[i];

            if (GridEntryMap.TryGetValue(neighborCoordinateID, out HeatData neighborData))
            {
                float neighborTemperature = neighborData.temperature;

                float deltaTemperature = neighborTemperature - currentTemperature;

                float temperatureGradient = deltaTemperature / cellSize;

                float heatFlux = gridData.chemicalMaterial.thermalConductivity * temperatureGradient;

                float volume = cellSize * cellSize * cellSize;

                float heatCapacity = gridData.chemicalMaterial.specificHeat * (gridData.chemicalMaterial.densityPerCubicCm * volume);

                float changeInTemperature = (heatFlux * DeltaTime) / heatCapacity;

                totalDeltaTemperature += changeInTemperature;
            }
        }

        DeltaTemperatures[data.ID] += totalDeltaTemperature;
    }
}

[BurstCompile]
public partial struct ApplyTemperatureJob : IJobEntity
{
    [ReadOnly] public NativeArray<float> TemperaturesToApply;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<HeatShaderValues> HeatShaderValuesFromEntity;

    public void Execute(ref HeatData data, Entity entity)
    {
        data.temperature += TemperaturesToApply[data.ID];

        HeatShaderValues heatShaderValue = HeatShaderValuesFromEntity[entity];

        heatShaderValue.value = data.temperature;

        HeatShaderValuesFromEntity[entity] = heatShaderValue;
    }
}
