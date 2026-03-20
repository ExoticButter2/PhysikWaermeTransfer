using System;
using UnityEngine;
using UnityEngine.Profiling;

public class HeatMapUpdater : MonoBehaviour
{
    [SerializeField]
    private HeatMap _heatMap;

    private static readonly int _baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private float _heatChangeNeededForVisual = 0.1f;

    public void UpdateHeatColor(Heat heatComponent, float heat, float lastHeat, MeshRenderer meshRenderer, MaterialPropertyBlock propertyBlock, bool forceUpdate)
    {
        if (Mathf.Abs(heat - lastHeat) <= _heatChangeNeededForVisual && !forceUpdate)
        {
            return;
        }

        heatComponent.heatBeforeVisualUpdate = heat;

        Profiler.BeginSample("Heat color update");

        float rightAmount = Mathf.Clamp01(heat / _heatMap.maxTemperature);

        Color heatColor = _heatMap.heatMapImage.GetPixelBilinear(Mathf.Clamp01(rightAmount), 0.5f);//x coordinate depends on temperature

        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(_baseColorId, heatColor);
        meshRenderer.SetPropertyBlock(propertyBlock);

        Profiler.EndSample();
    }
}
