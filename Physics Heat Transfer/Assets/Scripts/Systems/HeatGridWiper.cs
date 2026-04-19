using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
public partial struct HeatGridWiper : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (request, requestEntity) in SystemAPI.Query<ClearButtonRequest>().WithEntityAccess())
        {
            foreach (var (data, voxelEntity) in SystemAPI.Query<RefRO<HeatData>>().WithNone<Disabled>().WithEntityAccess())
            {
                ecb.AddComponent<Disabled>(voxelEntity);
            }

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
}
