using UnityEngine;

public class TemperatureUnitSelector : MonoBehaviour
{
    public void ChangeToCelsius()
    {
        TemperatureUIHandler.Instance.selectedTemperatureUnit = TemperatureUIHandler.TemperatureUnit.Celsius;
    }

    public void ChangeToKelvin()
    {
        TemperatureUIHandler.Instance.selectedTemperatureUnit = TemperatureUIHandler.TemperatureUnit.Kelvin;
    }

    public void ChangeToFahrenheit()
    {
        TemperatureUIHandler.Instance.selectedTemperatureUnit = TemperatureUIHandler.TemperatureUnit.Fahrenheit;
    }
}
