using NUnit.Framework;
using System;
using Unity.Mathematics;
using UnityEngine;

public class HeatMapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject cubeMaterialPrefab;

    [SerializeField]
    public int height = 5;
    [SerializeField]
    public int width = 8;
    [SerializeField]
    public int depth = 2;

    public HeatMapUpdater heatMapUpdater;

    [SerializeField]
    private ChemicalMaterial _materialData;

    [SerializeField]
    private GameObject _materialObject;

    [HideInInspector]
    public Heat[,,] heatGrid;

    public float cellSize = 0.1f;
    public float scaleFactor = 1.0f;

    private void Awake()
    {
        InitializeHeatGrid();
    }

    private void InitializeHeatGrid()
    {
        int idCounter = 0;

        heatGrid = new Heat[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GameObject cubeMaterial = Instantiate(cubeMaterialPrefab, new Vector3(x * cellSize * scaleFactor, y * cellSize * scaleFactor, z * cellSize * scaleFactor), Quaternion.identity, gameObject.transform);
                    Heat cubeHeatComponent = cubeMaterial.GetComponent<Heat>();
                    cubeMaterial.transform.localScale = new Vector3(cellSize * scaleFactor, cellSize * scaleFactor, cellSize * scaleFactor);

                    if (cubeHeatComponent == null)
                    {
                        Debug.LogWarning("No heat component found inside cube material!");
                        continue;
                    }

                    cubeHeatComponent.gridX = x;
                    cubeHeatComponent.gridY = y;
                    cubeHeatComponent.gridZ = z;

                    cubeHeatComponent.distancePerCubeInSquareCm = cellSize;
                    cubeHeatComponent.thermalConductivity = _materialData.thermalConductivity;
                    cubeHeatComponent.heatMapGenerator = this;
                    cubeHeatComponent.densityPerCubicCm = _materialData.densityPerCubicCm;
                    cubeHeatComponent.specificHeat = _materialData.specificHeat;

                    cubeHeatComponent.heatID = idCounter;

                    cubeMaterial.layer = LayerMask.NameToLayer("HeatComponent");

                    heatGrid[x, y, z] = cubeHeatComponent;

                    idCounter++;
                }
            }
        }
    }
}
