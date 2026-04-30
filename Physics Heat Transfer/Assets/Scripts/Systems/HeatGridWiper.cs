using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
public partial struct HeatGridWiper : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (request, requestEntity) in SystemAPI.Query<ClearButtonRequest>().WithEntityAccess())
        {
            bool parentIncluded = false;

            foreach (var (data, voxelEntity) in SystemAPI.Query<RefRO<HeatData>>().WithNone<Disabled>().WithEntityAccess())
            {
                if (!parentIncluded)
                {
                    Parent parentComponent = state.EntityManager.GetComponentData<Parent>(voxelEntity);
                    ecb.AddComponent<Disabled>(parentComponent.Value);
                    parentIncluded = true;
                }

                ecb.AddComponent<Disabled>(voxelEntity);
            }

            ResetGridVariables(ref state);
            ecb.DestroyEntity(requestEntity);
        }

        int destroyLimit = 400;
        int count = 0;

        foreach (var (data, disabledEntity) in SystemAPI.Query<RefRO<HeatData>>().WithAll<Disabled>().WithEntityAccess())
        {
            ecb.DestroyEntity(disabledEntity);
            count++;

            if (count >= destroyLimit) break;
        }
    }

    private void ResetGridVariables(ref SystemState state)
    {
        SystemHandle heatGridGeneratorHandle = state.World.GetExistingSystem<HeatGridGenerator>();
        ref HeatGridGenerator heatGridGenerator = ref state.WorldUnmanaged.GetUnsafeSystemRef<HeatGridGenerator>(heatGridGeneratorHandle);

        heatGridGenerator.Id = 0;
        heatGridGenerator.GridId = 0;

        heatGridGenerator.CoordinateIDToHeatDataHashmap.Clear();

        SystemHandle gridGeneratorHandle = state.World.GetExistingSystem<HeatGridGenerator>();
        HeatGridGenerator gridGeneratorSystem = state.WorldUnmanaged.GetUnsafeSystemRef<HeatGridGenerator>(gridGeneratorHandle);
        gridGeneratorSystem.GridIDToGridDataHashmap.Clear();
    }
}
