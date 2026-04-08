using TMPro;
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

        HeatMapGenerator.Instance.width = newWidth;
        _widthSliderTextLabel.text = $"Breite: {newWidth}";
    }

    private void OnHeightSliderValueChanged(float value)
    {
        int newHeight = (int)value;

        HeatMapGenerator.Instance.height = newHeight;
        _heightSliderTextLabel.text = $"Höhe: {newHeight}";
    }

    private void OnDepthSliderValueChanged(float value)
    {
        int newDepth = (int)value;

        HeatMapGenerator.Instance.depth = newDepth;
        _depthSliderTextLabel.text = $"Tiefe: {newDepth}";
    }
}
