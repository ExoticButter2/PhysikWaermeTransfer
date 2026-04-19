//using System;
//using TMPro;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class TemperatureUIHandler : MonoBehaviour
//{
//    public static TemperatureUIHandler Instance;

//    [SerializeField]
//    private TextMeshProUGUI _heatTextLabel;

//    public TextMeshProUGUI unitTextLabel;

//    [SerializeField]
//    private Canvas _heatUICanvas;

//    [SerializeField]
//    private float _xOffset = 0f;
//    [SerializeField]
//    private float _yOffset = 0f;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Update()
//    {
//        UpdateHeatTextLabelPosition();
//    }

//    public enum TemperatureUnit
//    {
//        Celsius,
//        Kelvin,
//        Fahrenheit
//    }

//    [HideInInspector]
//    public TemperatureUnit selectedTemperatureUnit = TemperatureUnit.Celsius;

//    private void EnableHeatUI()
//    {
//        _heatUICanvas.enabled = true;
//    }

//    private void DisableHeatUI()
//    {
//        _heatUICanvas.enabled = false;
//    }

//    private void UpdateHeatUI(float heat)
//    {
//        switch (selectedTemperatureUnit)
//        {
//            case TemperatureUnit.Celsius:
//                _heatTextLabel.text = $"{Math.Round(heat, 2)}°C";
//                break;

//            case TemperatureUnit.Kelvin:
//                _heatTextLabel.text = $"{Math.Round(heat + 273.15f, 2)}K";
//                break;

//            case TemperatureUnit.Fahrenheit:
//                _heatTextLabel.text = $"{Math.Round(heat * (9 / 5) + 32, 2)}F";
//                break;
//        }
//    }

//    private void UpdateHeatTextLabelPosition()
//    {
//        Vector2 mousePosition = Mouse.current.position.ReadValue();

//        _heatTextLabel.rectTransform.position = new Vector3(mousePosition.x + _xOffset, mousePosition.y + _yOffset, 0f);
//    }

//    private void OnEnable()
//    {
//        MouseRaycastSystem.OnUpdateHeatUI += UpdateHeatUI;

//        MouseRaycastSystem.OnEnableHeatUI += EnableHeatUI;
//        MouseRaycastSystem.OnDisableHeatUI += DisableHeatUI;
//    }

//    private void OnDisable()
//    {
//        MouseRaycastSystem.OnUpdateHeatUI -= UpdateHeatUI;

//        MouseRaycastSystem.OnEnableHeatUI -= EnableHeatUI;
//        MouseRaycastSystem.OnDisableHeatUI -= DisableHeatUI;
//    }
//}
