using AYellowpaper.SerializedCollections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class HeatMapGenerator : MonoBehaviour
{
    private int _idCounter = 0;

    public static HeatMapGenerator Instance;

    [HideInInspector]
    public List<GameObject> heatMapParents = new List<GameObject>();
    [HideInInspector]
    public List<HeatGridManager> heatGridDataList = new List<HeatGridManager>();

    [SerializedDictionary(keyName: "Material", valueName: "Prefab")]
    public SerializedDictionary<ChemicalMaterial, GameObject> chemicalMaterialPrefab = new SerializedDictionary<ChemicalMaterial, GameObject>();

    [SerializeField]
    public int height = 5;
    [SerializeField]
    public int width = 8;
    [SerializeField]
    public int depth = 2;

    [SerializeField]
    private ChemicalMaterial _materialData;

    [HideInInspector]
    private Heat[,,] _heatGrid;

    public float cellSize = 0.1f;
    public float scaleFactor = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeHeatGrid(ChemicalMaterial material, Vector3 parentPosition)
    {
        _idCounter = 0;

        GameObject materialParentObject = new GameObject();

        Rigidbody parentRigidbody = materialParentObject.AddComponent<Rigidbody>();

        materialParentObject.name = $"{material.englishMaterialName} block";
        parentRigidbody.position = parentPosition;
        parentRigidbody.isKinematic = true;
        parentRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        heatMapParents.Add(materialParentObject);

        _heatGrid = new Heat[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GameObject cubeMaterial = Instantiate(chemicalMaterialPrefab[material], materialParentObject.transform.position + new Vector3(x * cellSize * scaleFactor, y * cellSize * scaleFactor, z * cellSize * scaleFactor), Quaternion.identity, materialParentObject.transform);
                    Heat cubeHeatComponent = cubeMaterial.GetComponent<Heat>();
                    cubeMaterial.transform.localScale = new Vector3(cellSize * scaleFactor, cellSize * scaleFactor, cellSize * scaleFactor);

                    if (cubeHeatComponent == null)
                    {
                        Debug.LogWarning("No heat component found inside cube material!");
                        continue;
                    }

                    cubeHeatComponent.GridX = x;
                    cubeHeatComponent.GridY = y;
                    cubeHeatComponent.GridZ = z;

                    cubeHeatComponent.HeatID = _idCounter;

                    cubeMaterial.layer = LayerMask.NameToLayer("HeatComponent");

                    _heatGrid[x, y, z] = cubeHeatComponent;

                    if (HeatMapUpdater.Instance.heatMapEnabled)
                    {
                        HeatMapUpdater.Instance.SetToHeatMapMaterial(cubeHeatComponent);
                    }

                    _idCounter++;
                }
            }
        }

        HeatGridManager HeatGridManager = materialParentObject.AddComponent<HeatGridManager>();
        heatGridDataList.Add(HeatGridManager);
        int totalSize = width * height * depth;

        HeatGridManager.HeatGrid = _heatGrid;
        HeatGridManager.CellSize = cellSize;
        HeatGridManager.HeatGridWidth = width;
        HeatGridManager.HeatGridHeight = height;
        HeatGridManager.HeatGridDepth = depth;
        HeatGridManager.TotalGridSize = totalSize;

        HeatGridManager.GridTemperatures = new NativeArray<float>(totalSize, Allocator.Persistent);
        HeatGridManager.PendingDeltaTemperatures = new NativeArray<float>(totalSize, Allocator.Persistent);
        HeatGridManager.HeatDataArray = new NativeArray<HeatData>(totalSize, Allocator.Persistent);
        HeatGridManager.NeighborDataArray = new NativeArray<NeighborData>(totalSize * 6, Allocator.Persistent);
        HeatGridManager.ChemicalMaterial = material;
        HeatGridManager.heatIDToHeatComponent = new Heat[totalSize];

        foreach (Heat heatComponent in HeatGridManager.HeatGrid)
        {
            int heatID = heatComponent.HeatID;
            HeatGridManager.HeatDataArray[heatID] = new HeatData(heatID);

            HeatGridManager.HeatIDToHeatComponent[heatID] = heatComponent;

            for (int i = 0; i < 6; i++)
            {
                int nx = heatComponent.GridX, ny = heatComponent.GridY, nz = heatComponent.GridZ;
                if (i == 0) nx++; // right
                else if (i == 1) nx--; // left
                else if (i == 2) ny++; // up
                else if (i == 3) ny--; // down
                else if (i == 4) nz++; // front
                else if (i == 5) nz--; // back

                if (nx >= 0 && nx < width &&
                    ny >= 0 && ny < height &&
                    nz >= 0 && nz < depth)
                {
                    HeatGridManager.NeighborDataArray[heatID * 6 + i] = new NeighborData(nx * (height * depth) + ny * depth + nz);
                }
            }
        }

        //CollisionHandler parentCollisionHandler = materialParentObject.AddComponent<CollisionHandler>();
        //parentCollisionHandler.HeatGridManager = heatGridData;

        parentRigidbody.isKinematic = false;
    }

    public void ClearAllHeatGrids()
    {
        foreach (GameObject heatMapParent in heatMapParents)
        {
            Destroy(heatMapParent);
        }
        heatMapParents.Clear();
        heatGridDataList.Clear();
        _idCounter = 0;
    }
}
