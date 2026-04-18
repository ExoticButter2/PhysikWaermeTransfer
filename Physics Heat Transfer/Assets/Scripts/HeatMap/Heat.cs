using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public struct HeatData
{
    public HeatData(int heatID)
    {
        this.HeatID = heatID;
    }

    public int HeatID;
}

public struct NeighborData
{
    public NeighborData(int heatID)
    {
        this.HeatID = heatID;
    }

    public int HeatID;
}

[BurstCompile]
public struct CalculateDeltaTemperatureJob : IJobParallelFor
{
    public float ThermalConductivity;
    public float CellSize;
    public float HeatCapacity;

    public float DeltaTime;

    [ReadOnly]
    public NativeArray<NeighborData> NeighborDataArray;
    [ReadOnly]
    public NativeArray<float> GridTemperatures;

    public NativeArray<float> PendingDeltaTemperatures;

    public void Execute(int index)
    {
        float currentTemperature = GridTemperatures[index];
        float totalDeltaTemperature = 0f;

        for (int i = 0; i < 6; i++)
        {
            NeighborData neighborData = NeighborDataArray[index * 6 + i];

            float neighborTemperature = GridTemperatures[neighborData.HeatID];
            float deltaTemperature = neighborTemperature - currentTemperature;

            float temperatureGradient = deltaTemperature / CellSize;

            float heatFlux = ThermalConductivity * temperatureGradient;
            // Waermestrom = Waermeleitfaehigkeit * Temperaturgradient

            float changeInTemperature = (heatFlux * DeltaTime) / HeatCapacity;
            // deltaTemperatur = Q/m*c

            totalDeltaTemperature += changeInTemperature;
        }

        PendingDeltaTemperatures[index] += totalDeltaTemperature;
    }
}

[DefaultExecutionOrder(50)]
public class Heat : MonoBehaviour
{
    [HideInInspector]
    public HeatGridManager HeatGridManager;

    [HideInInspector]
    public int HeatID = 0;

    [HideInInspector]
    public MeshRenderer MeshRenderer;

    [HideInInspector]
    public int GridX = 0;
    [HideInInspector]
    public int GridY = 0;
    [HideInInspector]
    public int GridZ = 0;

    private float _absoluteZeroPoint = -273.15f;//in celsius

    [HideInInspector]
    public HeatGridManager heatGridManager;

    public float HeatP
    {
        get
        {
            return HeatValue;
        }
        
        set
        {
            if (value < _absoluteZeroPoint)
            {
                HeatValue = _absoluteZeroPoint;
            }
            else
            {
                HeatValue = value;
            }
        } 
    }

    public float HeatValue = 0f;

    private void InitializeVariables()
    {
        MeshRenderer = transform.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        InitializeVariables();
    }
}
