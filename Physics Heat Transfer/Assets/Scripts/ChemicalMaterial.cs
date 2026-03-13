using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Create Material")]
public class ChemicalMaterial : ScriptableObject
{
    public float heatConductivity = 1.0f;
    public float heatCapacity = 20f;//joules per kilo
}
