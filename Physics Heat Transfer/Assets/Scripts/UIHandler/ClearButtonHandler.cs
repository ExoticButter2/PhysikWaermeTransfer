using UnityEngine;

public class ClearButtonHandler : MonoBehaviour
{
    public void OnClearButtonClicked()
    {
        HeatMapGenerator.Instance.ClearAllHeatGrids();
    }
}
