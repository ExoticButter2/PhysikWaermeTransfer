using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HeatDataAuthoring : MonoBehaviour
{
    public int4 XYZWithID;

    public int chemicalMaterialID;
}

public struct HeatData : IComponentData
{
    public int4 XYZWithGridID;
    public int ID;

    public int chemicalMaterialID;
    public float temperature;
}

public class HeatDataBaker : Baker<HeatDataAuthoring>
{
    public override void Bake(HeatDataAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new HeatData { XYZWithGridID = new int4(-1, -1, -1, -1), chemicalMaterialID = authoring.chemicalMaterialID, temperature = 0f, ID = -1 });
    }
}
