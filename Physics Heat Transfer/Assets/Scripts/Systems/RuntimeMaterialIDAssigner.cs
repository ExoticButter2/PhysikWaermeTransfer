using Unity.Entities;
using Unity.Rendering;

public partial struct RuntimeMaterialIDAssigner : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var graphicsSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();

        if (!SystemAPI.TryGetSingletonEntity<GridData>(out Entity gridDataEntity))
            return;

        GridData gridData = SystemAPI.GetComponent<GridData>(gridDataEntity);

        var buffer = SystemAPI.GetBuffer<ChemicalMaterialRuntimeData>(gridDataEntity);
        int bufferLength = buffer.Length;

        for (int i = 0; i < bufferLength; i++)
        {
            var data = buffer[i];

            data.visualBatchMaterialID = graphicsSystem.RegisterMaterial(data.visualMaterial);
            data.heatMapBatchMaterialID = graphicsSystem.RegisterMaterial(data.heatMapMaterial);

            buffer[i] = data;
        }
    }
}
