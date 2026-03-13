using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Heat : MonoBehaviour
{
    public HeatMapGenerator heatMapGenerator;

    public float heat;

    //private Heat _upNeighbor;
    //private Heat _downNeighbor;
    //private Heat _leftNeighbor;
    //private Heat _rightNeighbor;
    //private Heat _frontNeighbor;
    //private Heat _backNeighbor;

    private List<Heat> _heatNeighbors = new List<Heat>();

    private void Start()
    {
        GetNeighbors();
    }

    private void GetNeighbors()
    {
        Heat[,,] heatGrid = heatMapGenerator.heatGrid;

        int xPos = (int)transform.position.x;
        int yPos = (int)transform.position.y;
        int zPos = (int)transform.position.z;

        int xMax = heatGrid.GetLength(0);
        int yMax = heatGrid.GetLength(1);
        int zMax = heatGrid.GetLength(2);

        if (xPos <= 0)
        {
            Debug.LogWarning("X position is invalid!");
            return;
        }

        if (yPos <= 0)
        {
            Debug.LogWarning("Y position is invalid!");
            return;
        }

        if (zPos <= 0)
        {
            Debug.LogWarning("Z position is invalid!");
            return;
        }

        //X
        Heat rightNeighbor = (xPos + 1 < xMax) ? heatGrid[xPos + 1, yPos, zPos] : null;
        Heat leftNeighbor = (xPos - 1 >= 0) ? heatGrid[xPos - 1, yPos, zPos] : null;

        //Y
        Heat upNeighbor = (yPos + 1 < yMax) ? heatGrid[xPos, yPos + 1, zPos] : null;
        Heat downNeighbor = (yPos - 1 >= 0) ? heatGrid[xPos, yPos - 1, zPos] : null;

        //Z
        Heat frontNeighbor = (zPos + 1 < zMax) ? heatGrid[xPos, yPos, zPos + 1] : null;
        Heat backNeighbor = (zPos - 1 >= 0) ? heatGrid[xPos, yPos, zPos - 1] : null;

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
    }

    public void SpreadHeat()
    {

    }
}
