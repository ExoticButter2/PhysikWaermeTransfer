using UnityEngine;

[CreateAssetMenu(fileName = "HeatMap", menuName = "Create new heat map")]
public class HeatMap : ScriptableObject
{
    public Texture2D heatMapImage;

    public double maxTemperature = 200f;
    public double minTemperature = 0f;
}
