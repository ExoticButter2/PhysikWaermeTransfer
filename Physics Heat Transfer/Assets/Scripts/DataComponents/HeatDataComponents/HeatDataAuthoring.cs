using Unity.Entities;
using UnityEngine;

public class HeatDataAuthoring : MonoBehaviour
{
    public int heatID;
    public int GridX;
    public int GridY;
    public int GridZ;

    public int chemicalMaterialID;
}

public struct HeatData : IComponentData
{
    public int heatID;
    public float GridX;
    public float GridY;
    public float GridZ;

    public int chemicalMaterialID;
}

public class HeatDataBaker : Baker<HeatDataAuthoring>
{
    public override void Bake(HeatDataAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new HeatData { heatID = -1, GridX = -1, GridY = -1, GridZ = -1, chemicalMaterialID = authoring.chemicalMaterialID });
    }
}
