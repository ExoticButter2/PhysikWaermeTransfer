using NUnit.Framework;
using System;
using UnityEngine;

public class HeatMapGenerator : MonoBehaviour
{
    [SerializeField]
    private Texture2D _heatMap;

    [SerializeField]
    private ChemicalMaterial _materialData;

    [SerializeField]
    private GameObject _materialObject;

    private Heat[,,] _heatGrid;

    [SerializeField]
    private float cellSize = 0.1f;

    private void Start()
    {
        InitializeHeatGrid();
        FillHeatGrid();
    }

    private void InitializeHeatGrid()
    {
        Bounds bounds = new Bounds(_materialObject.transform.position, Vector3.one);

        foreach (Transform child in _materialObject.transform)
        {
            bounds.Encapsulate(child.position);
        }

        Vector3Int gridSize = new Vector3Int(
            Mathf.CeilToInt(bounds.size.x / cellSize),
            Mathf.CeilToInt(bounds.size.y / cellSize),
            Mathf.CeilToInt(bounds.size.z / cellSize)
        );

        _heatGrid = new Heat[gridSize.x, gridSize.y, gridSize.z];
    }

    private void FillHeatGrid()
    {
        foreach (Transform child in _materialObject.transform)
        {
            Heat heatComponent = child.GetComponent<Heat>();

            if (heatComponent == null)
            {
                Debug.LogWarning("No heat component found inside child");
                continue;
            }

            Vector3 childPosition = child.position;

            _heatGrid[(int)childPosition.x, (int)childPosition.y, (int)childPosition.z] = heatComponent;
        }

        Debug.Log("Filled heat grid!");
    }
}
