using NUnit.Framework;
using System;
using Unity.Mathematics;
using UnityEngine;

public class HeatMapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject cubeMaterialPrefab;

    [SerializeField]
    public int heightWidth = 16;

    public HeatMapUpdater heatMapUpdater;

    [SerializeField]
    private ChemicalMaterial _materialData;

    [SerializeField]
    private GameObject _materialObject;

    [HideInInspector]
    public Heat[,,] heatGrid;

    [SerializeField]
    private float cellSize = 0.1f;

    private void Awake()
    {
        InitializeHeatGrid();
    }

    private void InitializeHeatGrid()
    {
        int idCounter = 0;

        heatGrid = new Heat[heightWidth, heightWidth, heightWidth];

        for (int x = 0; x < heightWidth; x++)
        {
            for (int y = 0; y < heightWidth; y++)
            {
                for (int z = 0; z < heightWidth; z++)
                {
                    GameObject cubeMaterial = Instantiate(cubeMaterialPrefab, new Vector3(x * cellSize, y * cellSize, z * cellSize), Quaternion.identity, gameObject.transform);
                    Heat cubeHeatComponent = cubeMaterial.GetComponent<Heat>();

                    if (cubeHeatComponent == null)
                    {
                        Debug.LogWarning("No heat component found inside cube material!");
                        continue;
                    }

                    cubeHeatComponent.gridX = x;
                    cubeHeatComponent.gridY = y;
                    cubeHeatComponent.gridZ = z;

                    cubeHeatComponent.heatID = idCounter;

                    cubeMaterial.layer = LayerMask.NameToLayer("HeatComponent");

                    heatGrid[x, y, z] = cubeHeatComponent;

                    idCounter++;
                }
            }
        }
    }
}
