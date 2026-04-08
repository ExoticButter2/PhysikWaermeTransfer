using UnityEngine;

public class TemperatureUnitSelector : MonoBehaviour
{
    public void ChangeToCelsius()
    {
        TemperatureUIHandler.Instance.unitTextLabel.text = $"Einheit: Celsius";
        TemperatureUIHandler.Instance.selectedTemperatureUnit = TemperatureUIHandler.TemperatureUnit.Celsius;
    }

    public void ChangeToKelvin()
    {
        TemperatureUIHandler.Instance.unitTextLabel.text = $"Einheit: Kelvin";
        TemperatureUIHandler.Instance.selectedTemperatureUnit = TemperatureUIHandler.TemperatureUnit.Kelvin;
    }

    public void ChangeToFahrenheit()
    {
        TemperatureUIHandler.Instance.unitTextLabel.text = $"Einheit: Fahrenheit";
        TemperatureUIHandler.Instance.selectedTemperatureUnit = TemperatureUIHandler.TemperatureUnit.Fahrenheit;
    }
}
