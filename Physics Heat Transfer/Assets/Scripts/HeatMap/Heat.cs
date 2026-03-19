using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Heat : MonoBehaviour
{
    public MaterialPropertyBlock propertyBlock;
    public MeshRenderer meshRenderer;

    public int gridX = 0;
    public int gridY = 0;
    public int gridZ = 0;

    public HeatMapGenerator heatMapGenerator;

    public float heat;

    //private Heat _upNeighbor;
    //private Heat _downNeighbor;
    //private Heat _leftNeighbor;
    //private Heat _rightNeighbor;
    //private Heat _frontNeighbor;
    //private Heat _backNeighbor;

    private List<Heat> _heatNeighbors = new List<Heat>();

    [SerializeField]
    private ChemicalMaterial _chemicalMaterial;

    private void Start()
    {
        meshRenderer = transform.GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        heatMapGenerator = transform.parent.GetComponent<HeatMapGenerator>();
        GetNeighbors();
    }

    private void Update()
    {
        SpreadHeat();
    }

    private void GetNeighbors()
    {
        Heat[,,] heatGrid = heatMapGenerator.heatGrid;

        Debug.Log($"Z position: {gridZ}");

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

        #region nullchecks
        if (rightNeighbor != null)
        {
            _heatNeighbors.Add(rightNeighbor);
        }

        if (leftNeighbor != null)
        {
            _heatNeighbors.Add(leftNeighbor);
        }

        if (upNeighbor != null)
        {
            _heatNeighbors.Add(upNeighbor);
        }

        if (downNeighbor != null)
        {
            _heatNeighbors.Add(downNeighbor);
        }

        if (frontNeighbor != null)
        {
            _heatNeighbors.Add(frontNeighbor);
        }

        if (backNeighbor != null)
        {
            _heatNeighbors.Add(backNeighbor);
        }
        #endregion

        Debug.Log($"Heat neighbors list size: {_heatNeighbors.Count}");
    }

    public void SpreadHeat()
    {
        int heatNeighborCount = _heatNeighbors.Count;
        Dictionary<Heat, float> temperatureChangeForNeighbors = new Dictionary<Heat, float>();

        foreach (Heat neighbor in _heatNeighbors)
        {
            float deltaTemperature = this.heat - neighbor.heat;

            float changeInTemperature = 0f;

            float heatFlux = _chemicalMaterial.heatConductivity * (deltaTemperature / _chemicalMaterial.distancePerCubeInSquareCm);//1 is 1^2cm distance, get heatflux for deltaT
            // Waermestromdichte = Waermeleitfaehigkeit * (deltaTemperatur / Distanz)

            changeInTemperature = (heatFlux * Time.deltaTime) / (_chemicalMaterial.densityPerCubicCm * _chemicalMaterial.specificHeat * _chemicalMaterial.distancePerCubeInSquareCm);
            // deltaTemperatur = (Waermestromdichte) / (Dichte * spezifische Waerme * Volumen (hier aber distanz weil es im modell wuerfeln sind))

            temperatureChangeForNeighbors.Add(neighbor, changeInTemperature);
        }

        foreach (KeyValuePair<Heat, float> changeForNeighbor in temperatureChangeForNeighbors)
        {
            Heat heatComponent = changeForNeighbor.Key;
            float deltaTemperature = changeForNeighbor.Value;

            heatComponent.heat += deltaTemperature;
            this.heat -= deltaTemperature;

            heatMapGenerator.heatMapUpdater.UpdateHeatColor(heatComponent.gameObject, heatComponent.heat, heatComponent.meshRenderer, heatComponent.propertyBlock);
            heatMapGenerator.heatMapUpdater.UpdateHeatColor(gameObject, this.heat, this.meshRenderer, this.propertyBlock);
            Debug.Log("Updated heat map!");
        }

        Debug.Log($"Amount of neighbors: {temperatureChangeForNeighbors.Count}");
    }
}
