using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class SizeSliderHandler : MonoBehaviour
{
    [SerializeField]
    private Slider _widthSlider;
    [SerializeField]
    private TextMeshProUGUI _widthSliderTextLabel;

    [SerializeField]
    private Slider _heightSlider;
    [SerializeField]
    private TextMeshProUGUI _heightSliderTextLabel;

    [SerializeField]
    private Slider _depthSlider;
    [SerializeField]
    private TextMeshProUGUI _depthSliderTextLabel;

    private void Start()
    {
        _widthSlider.onValueChanged.AddListener(OnWidthSliderValueChanged);
        _heightSlider.onValueChanged.AddListener(OnHeightSliderValueChanged);
        _depthSlider.onValueChanged.AddListener(OnDepthSliderValueChanged);
    }

    private void OnWidthSliderValueChanged(float value)
    {
        int newWidth = (int)value;

        //HeatMapGenerator.Instance.width = newWidth;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity queryEntity = entityManager.CreateEntityQuery(typeof(GridData)).GetSingletonEntity();

        GridData readGridData = entityManager.GetComponentData<GridData>(queryEntity);

        readGridData.width = (int)value;

        entityManager.SetComponentData(queryEntity, readGridData);
        _widthSliderTextLabel.text = $"Breite: {newWidth}";
    }

    private void OnHeightSliderValueChanged(float value)
    {
        int newHeight = (int)value;

        //HeatMapGenerator.Instance.height = newHeight;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity queryEntity = entityManager.CreateEntityQuery(typeof(GridData)).GetSingletonEntity();

        GridData readGridData = entityManager.GetComponentData<GridData>(queryEntity);

        readGridData.height = (int)value;

        entityManager.SetComponentData(queryEntity, readGridData);

        _heightSliderTextLabel.text = $"Höhe: {newHeight}";
    }

    private void OnDepthSliderValueChanged(float value)
    {
        int newDepth = (int)value;

        //HeatMapGenerator.Instance.depth = newDepth;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity queryEntity = entityManager.CreateEntityQuery(typeof(GridData)).GetSingletonEntity();

        GridData readGridData = entityManager.GetComponentData<GridData>(queryEntity);

        readGridData.depth = (int)value;

        entityManager.SetComponentData(queryEntity, readGridData);

        _depthSliderTextLabel.text = $"Tiefe: {newDepth}";
    }
}
