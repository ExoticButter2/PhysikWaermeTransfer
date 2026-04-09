using System.Collections.Generic;
using UnityEngine;

public class HeatGridData : MonoBehaviour
{
    [HideInInspector]
    public Heat[,,] _heatGrid;
    [HideInInspector]
    public Dictionary<int, Heat> heatIDToHeatComponent = new Dictionary<int, Heat>();
    [HideInInspector]
    public float _cellSize;
}
