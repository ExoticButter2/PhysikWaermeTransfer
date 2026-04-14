using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Profiling;
using UnityEngine.Profiling;
using System;

public class Heat : MonoBehaviour
{
    [HideInInspector]
    public int heatID = 0;

    [HideInInspector]
    public MeshRenderer meshRenderer;

    [HideInInspector]
    public int gridX = 0;
    [HideInInspector]
    public int gridY = 0;
    [HideInInspector]
    public int gridZ = 0;

    private float _absoluteZeroPoint = -273.15f;//in celsius

    private float _cellSize;
    private float _cubeVolume;
    private float _thermalConductivity;
    private float _densityPerCubicCm;
    private float _specificHeat;
    private float _heatCapacity;

    public float HeatP
    {
        get
        {
            return heatValue;
        }
        
        set
        {
            if (value < _absoluteZeroPoint)
            {
                heatValue = _absoluteZeroPoint;
            }
            else
            {
                heatValue = value;
            }
        } 
    }

    public float heatValue = 0f;
    [HideInInspector]
    public float heatBeforeVisualUpdate = 0f;

    [HideInInspector]
    public List<Heat> heatNeighbors = new List<Heat>();

    public ChemicalMaterial chemicalMaterial;

    [HideInInspector]
    public HeatGridData _heatGridData;

    private void InitializeVariables()
    {
        meshRenderer = transform.GetComponent<MeshRenderer>();

        _cellSize = _heatGridData._cellSize;
        _cubeVolume = _cellSize * _cellSize * _cellSize;
        _thermalConductivity = chemicalMaterial.thermalConductivity;
        _densityPerCubicCm = chemicalMaterial.densityPerCubicCm;
        _specificHeat = chemicalMaterial.specificHeat;
        _heatCapacity = (_densityPerCubicCm * _cubeVolume) * _specificHeat;
    }

    private void Start()
    {
        InitializeVariables();
        GetNeighbors();
    }

    private void Update()
    {
        SimulateHeat();
    }

    private void GetNeighbors()
    {
        Heat[,,] heatGrid = _heatGridData._heatGrid;

        int xMax = heatGrid.GetLength(0);
        int yMax = heatGrid.GetLength(1);
        int zMax = heatGrid.GetLength(2);

        if (gridX < 0)
        {
            Debug.LogWarning("X position is invalid!");
            return;
        }

        if (gridY < 0)
        {
            Debug.LogWarning("Y position is invalid!");
            return;
        }

        if (gridZ < 0)
        {
            Debug.LogWarning("Z position is invalid!");
            return;
        }

        //X
        Heat rightNeighbor = (gridX + 1 < xMax) ? heatGrid[gridX + 1, gridY, gridZ] : null;
        Heat leftNeighbor = (gridX - 1 >= 0) ? heatGrid[gridX - 1, gridY, gridZ] : null;

        //Y
        Heat upNeighbor = (gridY + 1 < yMax) ? heatGrid[gridX, gridY + 1, gridZ] : null;
        Heat downNeighbor = (gridY - 1 >= 0) ? heatGrid[gridX, gridY - 1, gridZ] : null;

        //Z
        Heat frontNeighbor = (gridZ + 1 < zMax) ? heatGrid[gridX, gridY, gridZ + 1] : null;
        Heat backNeighbor = (gridZ - 1 >= 0) ? heatGrid[gridX, gridY, gridZ - 1] : null;

        List<Heat> neighbors = new List<Heat>();

        #region to list adder
        if (rightNeighbor != null)
        {
            neighbors.Add(rightNeighbor);
        }

        if (leftNeighbor != null)
        {
            neighbors.Add(leftNeighbor);
        }

        if (upNeighbor != null)
        {
            neighbors.Add(upNeighbor);
        }

        if (downNeighbor != null)
        {
            neighbors.Add(downNeighbor);
        }

        if (frontNeighbor != null)
        {
            neighbors.Add(frontNeighbor);
        }

        if (backNeighbor != null)
        {
            neighbors.Add(backNeighbor);
        }

        heatNeighbors = new List<Heat>(neighbors);
        #endregion
    }

    private void SimulateHeat()
    {
        Profiler.BeginSample("Heat simulation");
        float totalDeltaTemperature = 0;

        int count = heatNeighbors.Count;

        for (int i = 0; i < count; i++)
        {
            Heat heat = heatNeighbors[i];
            float deltaTemperature = CalculateDeltaTemperature(heat);
            ApplyTemperatureTo(heat, deltaTemperature);
            totalDeltaTemperature += deltaTemperature;
        }

        ApplyTemperatureTo(this, -totalDeltaTemperature);
        Profiler.EndSample();
    }

    public float CalculateDeltaTemperature(Heat neighbor)
    {
        Profiler.BeginSample("Change in temperature calculation");
        float deltaTemperature = HeatP - neighbor.HeatP;

        float changeInTemperature = 0f;

        float temperatureGradient = deltaTemperature / _cellSize;

        float heatFlux = _thermalConductivity * temperatureGradient;
        // Waermestrom = Waermeleitfaehigkeit * Temperaturgradient

        changeInTemperature = (heatFlux * Time.deltaTime) / _heatCapacity;
        // deltaTemperatur = Q/m*c

        Profiler.EndSample();

        return changeInTemperature;
    }

    private void ApplyTemperatureTo(Heat heatComponent, float deltaTemperature)
    {
        heatComponent.HeatP += deltaTemperature;
        HeatMapUpdater.Instance.UpdateHeatColor(heatComponent, false);
    }
}
