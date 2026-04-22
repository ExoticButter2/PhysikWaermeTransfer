using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class SizeSliderHandler : MonoBehaviour
{
    public static SizeSliderHandler Instance;

    private LocalizationManager _localizationManager;

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

    public int width;
    public int height;
    public int depth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _localizationManager = LocalizationManager.Instance;
    }

    private void Start()
    {
        _widthSlider.onValueChanged.AddListener(OnWidthSliderValueChanged);
        _heightSlider.onValueChanged.AddListener(OnHeightSliderValueChanged);
        _depthSlider.onValueChanged.AddListener(OnDepthSliderValueChanged);
    }

    private void OnWidthSliderValueChanged(float value)
    {
        int newWidth = (int)value;
        width = newWidth;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (!entityManager.CreateEntityQuery(typeof(GridData)).TryGetSingletonEntity<GridData>(out Entity queryEntity))
        {
            Debug.LogWarning("No grid data singleton entity found!");
        }

        GridData readGridData = entityManager.GetComponentData<GridData>(queryEntity);

        readGridData.width = (int)value;

        entityManager.SetComponentData(queryEntity, readGridData);

        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _widthSliderTextLabel.text = $"Width: {newWidth}";
                break;

            case Languages.German:
                _widthSliderTextLabel.text = $"Breite: {newWidth}";
                break;

            case Languages.Bulgarian:
                _widthSliderTextLabel.text = $"Ширина: {newWidth}";
                break;
        }
    }

    private void OnHeightSliderValueChanged(float value)
    {
        int newHeight = (int)value;
        height = newHeight;

        //HeatMapGenerator.Instance.height = newHeight;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (!entityManager.CreateEntityQuery(typeof(GridData)).TryGetSingletonEntity<GridData>(out Entity queryEntity))
        {
            Debug.LogWarning("No grid data singleton entity found!");
        }

        GridData readGridData = entityManager.GetComponentData<GridData>(queryEntity);

        readGridData.height = (int)value;

        entityManager.SetComponentData(queryEntity, readGridData);

        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _heightSliderTextLabel.text = $"Height: {newHeight}";
                break;

            case Languages.German:
                _heightSliderTextLabel.text = $"Höhe: {newHeight}";
                break;

            case Languages.Bulgarian:
                _heightSliderTextLabel.text = $"Височина: {newHeight}";
                break;
        }
    }

    private void OnDepthSliderValueChanged(float value)
    {
        int newDepth = (int)value;
        depth = newDepth;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (!entityManager.CreateEntityQuery(typeof(GridData)).TryGetSingletonEntity<GridData>(out Entity queryEntity))
        {
            Debug.LogWarning("No grid data singleton entity found!");
        }

        GridData readGridData = entityManager.GetComponentData<GridData>(queryEntity);

        readGridData.depth = (int)value;

        entityManager.SetComponentData(queryEntity, readGridData);

        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _depthSliderTextLabel.text = $"Depth: {newDepth}";
                break;

            case Languages.German:
                _depthSliderTextLabel.text = $"Tiefe: {newDepth}";
                break;

            case Languages.Bulgarian:
                _depthSliderTextLabel.text = $"Дълбочина: {newDepth}";
                break;
        }
    }
}
