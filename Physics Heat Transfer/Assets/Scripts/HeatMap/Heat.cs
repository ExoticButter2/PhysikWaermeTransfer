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
    public float pendingDeltaTemperature = 0f;

    [HideInInspector]
    public MaterialPropertyBlock propertyBlock;
    [HideInInspector]
    public MeshRenderer meshRenderer;

    [HideInInspector]
    public int gridX = 0;
    [HideInInspector]
    public int gridY = 0;
    [HideInInspector]
    public int gridZ = 0;

    private float _absoluteZeroPoint = -273.15f;//in celsius

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

    private List<Heat> _heatNeighbors = new List<Heat>();

    public ChemicalMaterial chemicalMaterial;

    [HideInInspector]
    public HeatGridData _heatGridData;

    private void InitializeVariables()
    {
        meshRenderer = transform.GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        InitializeVariables();
        GetNeighbors();

        if (HeatMapUpdater.Instance.heatMapEnabled)
        {
            HeatMapUpdater.Instance.UpdateHeatColor(this, HeatP, heatBeforeVisualUpdate, meshRenderer, propertyBlock, true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Heat heatComponent = collision.gameObject.GetComponent<Heat>();

        if (heatComponent != null)
        {
            _heatNeighbors.Add(heatComponent);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Heat heatComponent = collision.gameObject.GetComponent<Heat>();

        if (heatComponent != null && _heatNeighbors.Contains(heatComponent))
        {
            _heatNeighbors.Remove(heatComponent);
        }
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

        _heatNeighbors = new List<Heat>(neighbors);
        #endregion
    }

    private void SimulateHeat()
    {
        float totalDeltaTemperature = 0;

        foreach (Heat heat in _heatNeighbors)
        {
            float deltaTemperature = CalculateDeltaTemperature(heat);
            heat.pendingDeltaTemperature = deltaTemperature;

            totalDeltaTemperature += deltaTemperature;
        }

        foreach (Heat heat in _heatNeighbors)
        {
            ApplyTemperatureTo(heat, heat.pendingDeltaTemperature);
            heat.pendingDeltaTemperature = 0f;
        }

        ApplyTemperatureTo(this, -totalDeltaTemperature);
    }

    public float CalculateDeltaTemperature(Heat neighbor)
    {
        Profiler.BeginSample("Change in temperature calculation");
        float deltaTemperature = HeatP - neighbor.HeatP;

        float changeInTemperature = 0f;

        float temperatureGradient = deltaTemperature / _heatGridData._cellSize;

        float heatFlux = chemicalMaterial.thermalConductivity * temperatureGradient;
        // Waermestrom = Waermeleitfaehigkeit * Temperaturgradient

        changeInTemperature = (heatFlux * Time.deltaTime) / (chemicalMaterial.densityPerCubicCm * chemicalMaterial.specificHeat * _heatGridData._cellSize);
        // deltaTemperatur = (Waermestromdichte) / (Dichte * spezifische Waerme * Volumen (hier aber distanz, da es im modell wuerfeln sind))

        Profiler.EndSample();

        return changeInTemperature;
    }

    private void ApplyTemperatureTo(Heat heatComponent, float deltaTemperature)
    {
        heatComponent.HeatP += deltaTemperature;

        if (HeatMapUpdater.Instance.heatMapEnabled)
        {
            HeatMapUpdater.Instance.UpdateHeatColor(heatComponent, heatComponent.HeatP, heatComponent.heatBeforeVisualUpdate, heatComponent.meshRenderer, heatComponent.propertyBlock, false);
        }
    }
}
