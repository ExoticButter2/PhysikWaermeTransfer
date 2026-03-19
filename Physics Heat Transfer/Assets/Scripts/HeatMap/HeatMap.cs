using UnityEngine;

[CreateAssetMenu(fileName = "HeatMap", menuName = "Create new heat map")]
public class HeatMap : ScriptableObject
{
    public Texture2D heatMapImage;

    public float maxTemperature = 200f;
    public float minTemperature = 0f;
}
