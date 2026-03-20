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
    public int heatID = 0;

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

    public HeatMapGenerator heatMapGenerator;

    public float heat = 0f;
    public float heatBeforeVisualUpdate = 0f;

    //private Heat _upNeighbor;
    //private Heat _downNeighbor;
    //private Heat _leftNeighbor;
    //private Heat _rightNeighbor;
    //private Heat _frontNeighbor;
    //private Heat _backNeighbor;

    private Heat[] _heatNeighbors;

    private float[] _temperatureChangeForNeighbors;

    [SerializeField]
    private ChemicalMaterial _chemicalMaterial;

    private float _heatConductivity = 0;
    private float _distancePerCubeInSquareCm = 0;
    private float _specificHeat = 0;
    private float _densityPerCubicCm = 0;

    private void InitializeVariables()
    {
        _heatConductivity = _chemicalMaterial.heatConductivity;
        _distancePerCubeInSquareCm = _chemicalMaterial.distancePerCubeInSquareCm;
        _specificHeat = _chemicalMaterial.specificHeat;
        _densityPerCubicCm = _chemicalMaterial.densityPerCubicCm;

        meshRenderer = transform.GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        heatMapGenerator = transform.parent.GetComponent<HeatMapGenerator>();
    }

    private void Start()
    {
        InitializeVariables();
        GetNeighbors();

        _temperatureChangeForNeighbors = new float[heatMapGenerator.heatGrid.Length];

        heatMapGenerator.heatMapUpdater.UpdateHeatColor(this, heat, heatBeforeVisualUpdate, meshRenderer, propertyBlock, true);
    }

    private void Update()
    {
        SpreadHeat();
    }

    private void GetNeighbors()
    {
        Heat[,,] heatGrid = heatMapGenerator.heatGrid;

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

        #region nullchecks
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

        _heatNeighbors = new Heat[neighbors.Count];

        for (int i = 0; i < neighbors.Count; i++)
        {
            _heatNeighbors[i] = neighbors[i];
        }
        #endregion
    }

    public void SpreadHeat()
    {
        Array.Clear(_temperatureChangeForNeighbors, 0, _temperatureChangeForNeighbors.Length);

        Profiler.BeginSample("Change in temperature calculation");
        foreach (Heat neighbor in _heatNeighbors)
        {
            float deltaTemperature = heat - neighbor.heat;

            float changeInTemperature = 0f;

            float heatFlux = _heatConductivity * (deltaTemperature / _distancePerCubeInSquareCm);//1 is 1^2cm distance, get heatflux for deltaT
            // Waermestromdichte = Waermeleitfaehigkeit * (deltaTemperatur / Distanz)

            changeInTemperature = (heatFlux * Time.deltaTime) / (_densityPerCubicCm * _specificHeat * _distancePerCubeInSquareCm);
            // deltaTemperatur = (Waermestromdichte) / (Dichte * spezifische Waerme * Volumen (hier aber distanz weil es im modell wuerfeln sind))

            _temperatureChangeForNeighbors[neighbor.heatID] = changeInTemperature;
        }
        Profiler.EndSample();

        Profiler.BeginSample("Application of temperature change (heat map visual and heat in component)");
        for (int i = 0; i < _heatNeighbors.Length; i++)
        {
            Heat heatComponent = _heatNeighbors[i];
            float deltaTemperature = _temperatureChangeForNeighbors[heatComponent.heatID];

            heatComponent.heat += deltaTemperature;
            this.heat -= deltaTemperature;

            heatMapGenerator.heatMapUpdater.UpdateHeatColor(heatComponent, heatComponent.heat, heatComponent.heatBeforeVisualUpdate, heatComponent.meshRenderer, heatComponent.propertyBlock, false);
        }

        heatMapGenerator.heatMapUpdater.UpdateHeatColor(this, heat, heatBeforeVisualUpdate, meshRenderer, propertyBlock, false);
        Profiler.EndSample();
    }
}
