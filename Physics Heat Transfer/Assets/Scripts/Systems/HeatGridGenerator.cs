using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct HeatGridParentData : IComponentData
{
    public GridData gridData;
    public int gridID;
}

[BurstCompile]
public partial struct HeatGridGenerator : ISystem
{
    private BufferLookup<ChemicalMaterialRuntimeData> _materialBufferLookup;
    private EntityQuery _gridDataQuery;
    public NativeParallelHashMap<int, GridData> GridIDToGridDataHashmap;
    public NativeParallelHashMap<int4, HeatData> CoordinateIDToHeatDataHashmap;

    public int Id;
    public int GridId;

    public void OnCreate(ref SystemState state)
    {
        EntityManager entityManager = state.World.EntityManager;

        Id = 0;
        GridId = 0;

        _materialBufferLookup = state.GetBufferLookup<ChemicalMaterialRuntimeData>();
        _gridDataQuery = state.GetEntityQuery(typeof(GridData));

        CoordinateIDToHeatDataHashmap = new NativeParallelHashMap<int4, HeatData>(1024, Allocator.Persistent);

        GridIDToGridDataHashmap = new NativeParallelHashMap<int, GridData>(2048, Allocator.Persistent);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(out var ecbSingleton))
            return;

        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        if (!SystemAPI.TryGetSingleton(out GridData gridData))
            return;

        Entity gridDataEntity = SystemAPI.GetSingletonEntity<GridData>();

        DynamicBuffer<ChemicalMaterialPrefabElement> chemicalPrefabBuffer = SystemAPI.GetBuffer<ChemicalMaterialPrefabElement>(gridDataEntity);

        Entity chemicalMaterialPrefabEntity = Entity.Null;

        foreach (ChemicalMaterialPrefabElement element in chemicalPrefabBuffer)
        {
            if (element.chemicalMaterialID == gridData.chemicalMaterial.id)
            {
                chemicalMaterialPrefabEntity = element.prefabEntity;
                break;
            }
        }

        if (chemicalMaterialPrefabEntity == Entity.Null)
            return;

        ChemicalMaterialBlobItem chemicalMaterial = gridData.chemicalMaterial;

        gridData.cellSize = 1;

        foreach (var (request, entity) in SystemAPI.Query<GenerateGridRequest>().WithEntityAccess())
        {
            int gridDataHashmapSize = GridIDToGridDataHashmap.Count() + 1;

            if (gridDataHashmapSize > GridIDToGridDataHashmap.Capacity)
            {
                GridIDToGridDataHashmap.Capacity = gridDataHashmapSize;
            }

            int heatDataHashmapSize = CoordinateIDToHeatDataHashmap.Count() + (gridData.width * gridData.height * gridData.depth) + 1;

            if (heatDataHashmapSize > CoordinateIDToHeatDataHashmap.Capacity)
            {
                CoordinateIDToHeatDataHashmap.Capacity = heatDataHashmapSize;
            }

            Entity parentEntity = SpawnGridParent(request.gridParentPosition, ecb, gridData);
            SpawnGrid(gridData, parentEntity, chemicalMaterialPrefabEntity, ecb, ref state);

            ecb.DestroyEntity(entity);
        }
    }

    [BurstCompile]
    private Entity SpawnGridParent(float3 position, EntityCommandBuffer ecb, GridData gridData)
    {
        Entity parentEntity = ecb.CreateEntity();
        ecb.AddComponent(parentEntity, new HeatGridParentData { gridData = gridData, gridID = GridId });
        ecb.AddComponent(parentEntity, LocalTransform.FromPosition(position));
        ecb.AddComponent<LocalToWorld>(parentEntity);

        return parentEntity;
    }

    [BurstCompile]
    private void SpawnGrid(GridData gridData, Entity parentEntity, Entity heatVoxelPrefabEntity, EntityCommandBuffer ecb, ref SystemState state)
    {
        state.Dependency.Complete();

        GridIDToGridDataHashmap.TryAdd(GridId, gridData);

        state.Dependency.Complete();

        for (int x = 0; x < gridData.width; x++)
        {
            for (int y = 0; y < gridData.height; y++)
            {
                for (int z = 0; z < gridData.depth; z++)
                {
                    SpawnHeatVoxel(new float3(x, y, z), heatVoxelPrefabEntity, parentEntity, gridData, ecb, ref state);
                    Id++;
                }
            }
        }
        GridId++;

        state.Dependency.Complete();
        CoordinateIDToHeatDataHashmap.Clear();

        UpdateHashmapHeatData updateHashmapHeatDataJob = new UpdateHashmapHeatData { parallelWriterHeatHashmap = CoordinateIDToHeatDataHashmap.AsParallelWriter() };
        state.Dependency = updateHashmapHeatDataJob.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    private void SpawnHeatVoxel(float3 localPosition, Entity entityToSpawn, Entity gridParentEntity, GridData gridData, EntityCommandBuffer ecb, ref SystemState state)
    {
        Entity spawnedEntity = ecb.Instantiate(entityToSpawn);

        ecb.RemoveComponent<SceneTag>(spawnedEntity);
        ecb.AddComponent(spawnedEntity, new Parent { Value = gridParentEntity });
        ecb.SetComponent(spawnedEntity, LocalTransform.FromPosition(localPosition));
        ecb.AddComponent(spawnedEntity, new HeatShaderValues { value = 0 });

        int4 heatVoxelCoordinate = new int4((int)localPosition.x, (int)localPosition.y, (int)localPosition.z, GridId);

        HeatData heatData = new HeatData
        {
            chemicalMaterialID = gridData.chemicalMaterial.id,
            XYZWithGridID = heatVoxelCoordinate,
            temperature = 0f,
            ID = Id
        };

        ecb.SetComponent(spawnedEntity, heatData);

        Entity gridDataEntity = _gridDataQuery.GetSingletonEntity();
        _materialBufferLookup.Update(ref state);

        var buffer = _materialBufferLookup[gridDataEntity];

        int bufferLength = buffer.Length;

        for (int i = 0; i < bufferLength; i++)
        {
            var bufferData = buffer[i];

            if (bufferData.chemicalMaterialID != gridData.chemicalMaterial.id)
                continue;
        }
    }

    public void OnDestroy(ref SystemState state)
    {
        if (CoordinateIDToHeatDataHashmap.IsCreated)
        {
            CoordinateIDToHeatDataHashmap.Dispose();
        }

        if (GridIDToGridDataHashmap.IsCreated)
        {
            GridIDToGridDataHashmap.Dispose();
        }
    }
}

[BurstCompile]
public partial struct UpdateHashmapHeatData : IJobEntity
{
    public NativeParallelHashMap<int4, HeatData>.ParallelWriter parallelWriterHeatHashmap;

    public void Execute(in HeatData data)
    {
        parallelWriterHeatHashmap.TryAdd(data.XYZWithGridID, data);
    }
}