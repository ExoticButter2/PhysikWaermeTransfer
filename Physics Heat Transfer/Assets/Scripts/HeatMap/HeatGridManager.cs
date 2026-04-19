//using System.Collections.Generic;
//using Unity.Collections;
//using Unity.Jobs;
//using UnityEngine;
//using UnityEngine.Profiling;

//[DefaultExecutionOrder(-50)]
//public class HeatGridManager : MonoBehaviour
//{
//    [HideInInspector]
//    public Heat[] heatIDToHeatComponent;

//    [HideInInspector]
//    public NativeArray<float> PendingDeltaTemperatures;
//    [HideInInspector]
//    public NativeArray<float> GridTemperatures;
//    [HideInInspector]
//    public NativeArray<HeatData> HeatDataArray;
//    [HideInInspector]
//    public NativeArray<NeighborData> NeighborDataArray;

//    [HideInInspector]
//    public Heat[,,] HeatGrid;
//    [HideInInspector]
//    public int HeatGridWidth;
//    [HideInInspector]
//    public int HeatGridHeight;
//    [HideInInspector]
//    public int HeatGridDepth;
//    [HideInInspector]
//    public int TotalGridSize;
//    [HideInInspector]
//    public Dictionary<int, Heat> HeatIDToHeatComponent = new Dictionary<int, Heat>();
//    [HideInInspector]
//    public float CellSize;

//    //[HideInInspector]
//    //public ChemicalMaterial ChemicalMaterial;
//    [HideInInspector]
//    public float CubeVolume;
//    [HideInInspector]
//    public float ThermalConductivity;
//    [HideInInspector]
//    public float DensityPerCubicCm;
//    [HideInInspector]
//    public float SpecificHeat;
//    [HideInInspector]
//    public float HeatCapacity;

//    private void InitializeVariables()
//    {
//        CubeVolume = CellSize * CellSize * CellSize;
//        ThermalConductivity = ChemicalMaterial.thermalConductivity;
//        DensityPerCubicCm = ChemicalMaterial.densityPerCubicCm;
//        SpecificHeat = ChemicalMaterial.specificHeat;
//        HeatCapacity = DensityPerCubicCm * CubeVolume * SpecificHeat;
//    }

//    private void Start()
//    {
//        InitializeVariables();
//    }

//    private void Update()
//    {
//        SimulateGrid();
//    }

//    private void CalculateGridTemperatures()
//    {
//        CalculateDeltaTemperatureJob calculateDeltaTemperatureJob = new CalculateDeltaTemperatureJob
//        {
//            ThermalConductivity = ThermalConductivity,
//            GridTemperatures = GridTemperatures,
//            NeighborDataArray = NeighborDataArray,
//            PendingDeltaTemperatures = PendingDeltaTemperatures,
//            CellSize = CellSize,
//            DeltaTime = Time.deltaTime,
//            HeatCapacity = HeatCapacity
//        };

//        JobHandle calculationHandle = calculateDeltaTemperatureJob.Schedule(HeatDataArray.Length, 128);

//        calculationHandle.Complete();
//    }

//    private void SimulateGrid()
//    {
//        Profiler.BeginSample("Heat simulation");
//        CalculateGridTemperatures();

//        for (int i = 0; i < TotalGridSize; i++)
//        {
//            Heat heat = HeatIDToHeatComponent[i];
//            int heatID = heat.HeatID;

//            heat.HeatP += PendingDeltaTemperatures[heatID];
//            PendingDeltaTemperatures[heatID] = 0f;
//            GridTemperatures[heatID] = heat.HeatP;

//            HeatMapUpdater.Instance.UpdateHeatColor(heat, false);
//        }
//        Profiler.EndSample();
//    }

//    private void OnDestroy()
//    {
//        if (GridTemperatures.IsCreated)
//        {
//            GridTemperatures.Dispose();
//        }

//        if (HeatDataArray.IsCreated)
//        {
//            HeatDataArray.Dispose();
//        }

//        if (NeighborDataArray.IsCreated)
//        {
//            NeighborDataArray.Dispose();
//        }

//        if (PendingDeltaTemperatures.IsCreated)
//        {
//            PendingDeltaTemperatures.Dispose();
//        }
//    }
//}
