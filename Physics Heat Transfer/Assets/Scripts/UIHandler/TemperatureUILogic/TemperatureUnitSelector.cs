using TMPro;
using Unity.Entities;
using UnityEngine;

public enum TemperatureUnit
{
    Celsius,
    Kelvin,
    Fahrenheit
}

public class TemperatureUnitSelector : MonoBehaviour
{
    public static TemperatureUnitSelector Instance;
    private LocalizationManager _localizationManager;

    [HideInInspector]
    public TemperatureUnit selectedTemperatureUnit = TemperatureUnit.Celsius;

    [SerializeField]
    private TextMeshProUGUI _unitTextLabel;

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

    public void ChangeToCelsius()
    {
        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _unitTextLabel.text = $"Measurement unit: Celsius";
                break;

            case Languages.German:
                _unitTextLabel.text = $"Maßeinheit: Celsius";
                break;

            case Languages.Bulgarian:
                _unitTextLabel.text = $"Мерна единица: Целзий";
                break;
        }

        selectedTemperatureUnit = TemperatureUnit.Celsius;
    }

    public void ChangeToKelvin()
    {
        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _unitTextLabel.text = $"Measurement unit: Kelvin";
                break;

            case Languages.German:
                _unitTextLabel.text = $"Maßeinheit: Kelvin";
                break;

            case Languages.Bulgarian:
                _unitTextLabel.text = $"Мерна единица: Келвин";
                break;
        }

        selectedTemperatureUnit = TemperatureUnit.Kelvin;
    }

    public void ChangeToFahrenheit()
    {
        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _unitTextLabel.text = $"Measurement unit: Fahrenheit";
                break;

            case Languages.German:
                _unitTextLabel.text = $"Maßeinheit: Fahrenheit";
                break;

            case Languages.Bulgarian:
                _unitTextLabel.text = $"Мерна единица: Фаренхайт";
                break;
        }

        selectedTemperatureUnit = TemperatureUnit.Fahrenheit;
    }
}
