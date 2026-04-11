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

    private uint[] _heatColorLUT;
    [SerializeField]
    private int _lookUpTableSize = 256;

    private void Start()
    {
        _heatColorLUT = new uint[_lookUpTableSize];

        for (int i = 0; i < _lookUpTableSize; i++)
        {
            float rightAmount = (float)i / (_lookUpTableSize - 1);
            Color heatColor = _heatMap.heatMapImage.GetPixelBilinear(Mathf.Clamp01(rightAmount), 0.5f);
            _heatColorLUT[i] = ((uint)(heatColor.a * 255) << 24) | ((uint)(heatColor.b * 255) << 16) | ((uint)(heatColor.g * 255) << 8) | ((uint)(heatColor.r * 255) << 0);
        }
    }

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
        double lastHeat = heatComponent.heatBeforeVisualUpdate;
        double heat = heatComponent.heatValue;

        if (renderer == null)
        {
            Debug.LogWarning("No renderer inside heat component!");
            return;
        }

        if ((Mathf.Abs((float)heat - (float)lastHeat) <= _heatChangeNeededForVisual || !heatMapEnabled) && !forceUpdate)
        {
            return;
        }

        heatComponent.heatBeforeVisualUpdate = heat;

        Profiler.BeginSample("Heat color update");

        float rightAmount = Mathf.Clamp01((float)heat / (float)_heatMap.maxTemperature);//x coordinate depends on temperature

        int index = Mathf.Clamp((int)(rightAmount * (_lookUpTableSize - 1)), 0, _lookUpTableSize - 1);

        uint heatMapColor = _heatColorLUT[index];

        renderer.SetShaderUserValue(heatMapColor);

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
