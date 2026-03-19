using System;
using UnityEngine;

public class HeatMapUpdater : MonoBehaviour
{
    [SerializeField]
    private HeatMap _heatMap;

    public void UpdateHeatColor(GameObject objectToChange, float heat, MeshRenderer meshRenderer, MaterialPropertyBlock propertyBlock)
    {
        Color heatColor = _heatMap.heatMapImage.GetPixelBilinear(Mathf.Clamp01(heat / _heatMap.maxTemperature), 0.5f);//x coordinate depends on temperature
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", heatColor);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
}
