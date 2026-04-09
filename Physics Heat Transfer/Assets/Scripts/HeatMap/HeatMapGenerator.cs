using AYellowpaper.SerializedCollections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HeatMapGenerator : MonoBehaviour
{
    private int _idCounter = 0;

    public static HeatMapGenerator Instance;

    [HideInInspector]
    public List<GameObject> heatMapParents = new List<GameObject>();
    [HideInInspector]
    public List<HeatGridData> heatGridDataList = new List<HeatGridData>();
    [HideInInspector]
    public Dictionary<int, Heat> heatIDToHeatComponent = new Dictionary<int, Heat>();

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
    public Heat[,,] heatGrid;

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
        GameObject materialParentObject = new GameObject();

        Rigidbody parentRigidbody = materialParentObject.AddComponent<Rigidbody>();

        materialParentObject.name = $"{material.englishMaterialName} block";
        parentRigidbody.position = parentPosition;
        parentRigidbody.isKinematic = true;
        parentRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        heatMapParents.Add(materialParentObject);

        heatGrid = new Heat[width, height, depth];

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

                    cubeHeatComponent.gridX = x;
                    cubeHeatComponent.gridY = y;
                    cubeHeatComponent.gridZ = z;

                    cubeHeatComponent.heatID = _idCounter;
                    heatIDToHeatComponent.Add(cubeHeatComponent.heatID, cubeHeatComponent);

                    cubeMaterial.layer = LayerMask.NameToLayer("HeatComponent");

                    heatGrid[x, y, z] = cubeHeatComponent;

                    if (HeatMapUpdater.Instance.heatMapEnabled)
                    {
                        HeatMapUpdater.Instance.SetToHeatMapMaterial(cubeHeatComponent);
                    }

                    _idCounter++;
                }
            }
        }

        HeatGridData heatGridData = materialParentObject.AddComponent<HeatGridData>();
        heatGridDataList.Add(heatGridData);

        heatGridData.heatIDToHeatComponent = heatIDToHeatComponent;
        heatGridData._heatGrid = heatGrid;
        heatGridData._cellSize = cellSize;

        foreach (Heat heatComponent in heatGridData._heatGrid)
        {
            heatComponent._heatGridData = heatGridData;
        }

        CollisionHandler parentCollisionHandler = materialParentObject.AddComponent<CollisionHandler>();
        parentCollisionHandler.heatGridData = heatGridData;

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
    }
}
