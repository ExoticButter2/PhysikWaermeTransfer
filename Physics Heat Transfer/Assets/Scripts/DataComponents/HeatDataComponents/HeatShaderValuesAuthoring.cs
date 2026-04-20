using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_Temperature")]
public struct HeatShaderValues : IComponentData
{
    public float value;
}
