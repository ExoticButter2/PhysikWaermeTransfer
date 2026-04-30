using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Temperature")]
public struct HeatShaderValues : IComponentData
{
    public float value;
}
