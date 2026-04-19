using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct HeatGridParentData : IComponentData
{
    public GridData gridData;
}

[BurstCompile]
public partial struct HeatGridGenerator : ISystem
{
    private BufferLookup<ChemicalMaterialRuntimeData> _materialBufferLookup;
    private EntityQuery _gridDataQuery;

    public void OnCreate(ref SystemState state)
    {
        _materialBufferLookup = state.GetBufferLookup<ChemicalMaterialRuntimeData>();
        _gridDataQuery = state.GetEntityQuery(typeof(GridData));
    }

    public void OnUpdate(ref SystemState state)
    {
        

        if (!SystemAPI.TryGetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(out var ecbSingleton))
            return;

        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        if (!SystemAPI.TryGetSingleton(out GridData gridData))
            return;

        Entity gridDataEntity = SystemAPI.GetSingletonEntity<GridData>();

        DynamicBuffer<ChemicalMaterialPrefabElement> chemicalPrefabBuffer = SystemAPI.GetBuffer<ChemicalMaterialPrefabElement>(gridDataEntity);

        Entity chemicalMaterialPrefabEntity = Entity.Null;

        foreach (ChemicalMaterialPrefabElement element in chemicalPrefabBuffer)
        {
            if (element.chemicalMaterialID == gridData.chemicalMaterial.id)
            {
                chemicalMaterialPrefabEntity = element.prefabEntity;
                break;
            }
        }

        if (chemicalMaterialPrefabEntity == Entity.Null)
            return;

        ChemicalMaterialBlobItem chemicalMaterial = gridData.chemicalMaterial;

        foreach (var (request, entity) in SystemAPI.Query<GenerateGridRequest>().WithEntityAccess())
        {
            Entity parentEntity = SpawnGridParent(request.gridParentPosition, ecb, gridData);
            SpawnGrid(gridData, parentEntity, chemicalMaterialPrefabEntity, ecb, ref state);

            ecb.DestroyEntity(entity);
        }
    }

    [BurstCompile]
    private Entity SpawnGridParent(float3 position, EntityCommandBuffer ecb, GridData gridData)
    {
        Entity parentEntity = ecb.CreateEntity();
        ecb.AddComponent(parentEntity, new HeatGridParentData { gridData = gridData });
        ecb.AddComponent(parentEntity, LocalTransform.FromPosition(position));
        ecb.AddComponent<LocalToWorld>(parentEntity);

        return parentEntity;
    }

    [BurstCompile]
    private void SpawnGrid(GridData gridData, Entity parentEntity, Entity heatVoxelPrefabEntity, EntityCommandBuffer ecb, ref SystemState state)
    {
        int id = 0;

        for (int x = 0; x < gridData.width; x++)
        {
            for (int y = 0; y < gridData.height; y++)
            {
                for (int z = 0; z < gridData.width; z++)
                {
                    SpawnHeatVoxel(new float3(x, y, z), heatVoxelPrefabEntity, parentEntity, gridData, id, ecb, ref state);
                    id++;
                }
            }
        }
    }

    [BurstCompile]
    private void SpawnHeatVoxel(float3 localPosition, Entity entityToSpawn, Entity gridParentEntity, GridData gridData, int id, EntityCommandBuffer ecb, ref SystemState state)
    {
        Entity spawnedEntity = ecb.Instantiate(entityToSpawn);

        ecb.RemoveComponent<SceneTag>(spawnedEntity);
        ecb.AddComponent(spawnedEntity, new Parent { Value = gridParentEntity });
        ecb.SetComponent(spawnedEntity, LocalTransform.FromPosition(localPosition));

        ecb.SetComponent(spawnedEntity, new HeatData
        {
            chemicalMaterialID = gridData.chemicalMaterial.id,
            GridX = localPosition.x,
            GridY = localPosition.y,
            GridZ = localPosition.z,
            heatID = id
        });

        Entity gridDataEntity = _gridDataQuery.GetSingletonEntity();
        _materialBufferLookup.Update(ref state);

        var buffer = _materialBufferLookup[gridDataEntity];

        int bufferLength = buffer.Length;

        for (int i = 0; i < bufferLength; i++)
        {
            var bufferData = buffer[i];

            if (bufferData.chemicalMaterialID != gridData.chemicalMaterial.id)
                continue;

            Entity requestEntity = ecb.CreateEntity();
            ecb.AddComponent(spawnedEntity, new HeatMapUpdateRequest { materialID = bufferData.visualBatchMaterialID, defaultHeatMapMaterialID = bufferData.heatMapBatchMaterialID });
        }
    }
}
