using System;
using UnityEngine;
using UnityEngine.Profiling;

public class HeatMapUpdater : MonoBehaviour
{
    public bool heatMapEnabled = false;

    public static HeatMapUpdater Instance;

    [SerializeField]
    private HeatMap _heatMap;

    private static readonly int _baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private float _heatChangeNeededForVisual = 0.1f;

    [SerializeField]
    private Material _heatMapMaterial;

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

    public void UpdateHeatColor(Heat heatComponent, bool forceUpdate)
    {
        MeshRenderer renderer = heatComponent.meshRenderer;
        MaterialPropertyBlock propertyBlock = heatComponent.propertyBlock;
        double lastHeat = heatComponent.heatBeforeVisualUpdate;
        double heat = heatComponent.heatValue;

        if (renderer == null)
        {
            Debug.LogWarning("No renderer inside heat component!");
            return;
        }

        if (propertyBlock == null)
        {
            Debug.LogWarning("No property block inside heat component!");
            return;
        }

        if ((Mathf.Abs((float)heat - (float)lastHeat) <= _heatChangeNeededForVisual || !heatMapEnabled) && !forceUpdate)
        {
            return;
        }

        heatComponent.heatBeforeVisualUpdate = heat;

        Profiler.BeginSample("Heat color update");

        float rightAmount = Mathf.Clamp01((float)heat / (float)_heatMap.maxTemperature);

        Color heatColor = _heatMap.heatMapImage.GetPixelBilinear(Mathf.Clamp01(rightAmount), 0.5f);//x coordinate depends on temperature

        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(_baseColorId, heatColor);
        renderer.SetPropertyBlock(propertyBlock);

        Profiler.EndSample();
    }

    public void ToggleHeatMapMode(bool mode)
    {
        heatMapEnabled = mode;

        if (mode)
        {
            foreach (HeatGridData heatGridData in HeatMapGenerator.Instance.heatGridDataList)
            {
                foreach (Heat heatComponent in heatGridData._heatGrid)
                {
                    SetToHeatMapMaterial(heatComponent);
                    UpdateHeatColor(heatComponent, true);
                }
            }
        }
        else
        {
            foreach (HeatGridData heatGridData in HeatMapGenerator.Instance.heatGridDataList)
            {
                foreach (Heat heatComponent in heatGridData._heatGrid)
                {
                    SetToNormalMaterial(heatComponent);
                }
            }
        }
    }

    public void SetToNormalMaterial(Heat heatObjectComponent)
    {
        MeshRenderer renderer = heatObjectComponent.gameObject.GetComponent<MeshRenderer>();

        if (renderer == null)
        {
            Debug.LogWarning("No renderer found inside heat object");
            return;
        }

        renderer.SetPropertyBlock(null);
        renderer.material = heatObjectComponent.chemicalMaterial.visualMaterial;

        heatObjectComponent.propertyBlock = new MaterialPropertyBlock();
    }

    public void SetToHeatMapMaterial(Heat heatObjectComponent)
    {
        MeshRenderer renderer = heatObjectComponent.gameObject.GetComponent<MeshRenderer>();

        if (renderer == null)
        {
            Debug.LogWarning("No renderer found inside heat object");
            return;
        }

        renderer.SetPropertyBlock(null);
        renderer.material = _heatMapMaterial;

        heatObjectComponent.propertyBlock = new MaterialPropertyBlock();

        UpdateHeatColor(heatObjectComponent, true);
    }
}
