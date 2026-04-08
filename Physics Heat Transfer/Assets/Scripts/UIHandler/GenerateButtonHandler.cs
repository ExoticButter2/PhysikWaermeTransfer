using UnityEngine;

public class GenerateButtonHandler : MonoBehaviour
{
    private Transform _cameraTransform;

    [SerializeField]
    private float distanceFromCameraToHeatGrid = 5f;

    public void OnGenerateButtonClicked()
    {
        _cameraTransform = Camera.main.transform;

        Vector3 heatGridParentPosition = _cameraTransform.position + _cameraTransform.forward * distanceFromCameraToHeatGrid;

        HeatMapGenerator.Instance.InitializeHeatGrid(MaterialPresetSelector.Instance.selectedMaterialPreset, heatGridParentPosition);
    }
}
