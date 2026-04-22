using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TemperatureUIHandler : MonoBehaviour
{
    public static TemperatureUIHandler Instance;
    private TemperatureUnitSelector _unitSelectorInstance;

    [SerializeField]
    private TextMeshProUGUI _heatTextLabel;

    [SerializeField]
    private float _xOffset = 0f;
    [SerializeField]
    private float _yOffset = 0f;

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
    }

    private void Start()
    {
        _unitSelectorInstance = TemperatureUnitSelector.Instance;
    }

    private void Update()
    {
        UpdateHeatTextLabelPosition();
    }

    private void EnableHeatUI()
    {
        _heatTextLabel.enabled = true;
    }

    private void DisableHeatUI()
    {
        _heatTextLabel.enabled = false;
    }

    private void UpdateHeatUI(float heat)
    {
        EnableHeatUI();

        switch (_unitSelectorInstance.selectedTemperatureUnit)
        {
            case TemperatureUnit.Celsius:
                _heatTextLabel.text = $"{Math.Round(heat - 273.15f, 2)}°C";
                break;

            case TemperatureUnit.Kelvin:
                _heatTextLabel.text = $"{Math.Round(heat, 2)}K";
                break;

            case TemperatureUnit.Fahrenheit:
                _heatTextLabel.text = $"{Math.Round((heat - 273.15f) * 1.8f + 32, 2)}F";
                break;
        }
    }

    private void UpdateHeatTextLabelPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        _heatTextLabel.rectTransform.position = new Vector3(mousePosition.x + _xOffset, mousePosition.y + _yOffset, 0f);
    }

    private void OnEnable()
    {
        TemperatureLabelSystem.OnUpdateTemperatureUI += UpdateHeatUI;
        TemperatureLabelSystem.OnTemperatureRaycastExit += DisableHeatUI;
    }

    private void OnDisable()
    {
        TemperatureLabelSystem.OnUpdateTemperatureUI -= UpdateHeatUI;
        TemperatureLabelSystem.OnTemperatureRaycastExit -= DisableHeatUI;
    }
}
