using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Create Material")]
public class ChemicalMaterial : ScriptableObject
{
    public float heatConductivity = 1.0f;
    public float distancePerCubeInSquareCm = 1.0f;
    public float densityPerCubicCm = 7.87f;
    public float specificHeat = 0.45f;//joules per gramm for 1 degree increase in temperature
}
