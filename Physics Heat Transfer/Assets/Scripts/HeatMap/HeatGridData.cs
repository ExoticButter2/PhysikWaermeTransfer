using UnityEngine;

public class HeatGridData : MonoBehaviour
{
    [HideInInspector]
    public Heat[,,] _heatGrid;
    [HideInInspector]
    public float _cellSize;
}
