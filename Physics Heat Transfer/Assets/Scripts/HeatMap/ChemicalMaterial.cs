using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Create Material")]
public class ChemicalMaterial : ScriptableObject
{
    public float thermalConductivity = 0.804f;//in W/(cm*K)
    public float densityPerCubicCm = 7.87f;
    public float specificHeat = 0.45f;//joules per gramm for 1 degree increase in temperature
}
