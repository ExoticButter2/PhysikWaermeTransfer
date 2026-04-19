using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

public struct HeatMapUpdateRequest : IComponentData 
{
    public BatchMaterialID materialID;
    public BatchMaterialID defaultHeatMapMaterialID;
}

public partial struct HeatMapMaterialUpdater : ISystem
{
    private bool _heatMapEnabled;
    private bool _lastState;

    private void SetToNormalMaterialGlobal(ref SystemState state)
    {
        Entity gridDataEntity = SystemAPI.GetSingletonEntity<GridData>();
        var buffer = SystemAPI.GetBuffer<ChemicalMaterialRuntimeData>(gridDataEntity);

        int bufferLength = buffer.Length;

        foreach (var (gridParentData, parentEntity) in SystemAPI.Query<HeatGridParentData>().WithEntityAccess())
        {
            for (int i = 0; i < bufferLength; i++)
            {
                var data = buffer[i];

                if (data.chemicalMaterialID != gridParentData.gridData.chemicalMaterial.id)
                    continue;

                BatchMaterialID materialID = data.visualBatchMaterialID;

                foreach (var (meshInfo, child) in SystemAPI.Query<RefRW<MaterialMeshInfo>, RefRO<Parent>>())
                {
                    if (child.ValueRO.Value == parentEntity)
                    {
                        var info = meshInfo.ValueRO;
                        info.MaterialID = materialID;
                        meshInfo.ValueRW = info;
                    }
                }
            }
        }
    }

    private void SetToHeatMapMaterialGlobal(ref SystemState state)
    {
        Entity gridDataEntity = SystemAPI.GetSingletonEntity<GridData>();
        var buffer = SystemAPI.GetBuffer<ChemicalMaterialRuntimeData>(gridDataEntity);

        int bufferLength = buffer.Length;

        foreach (var (gridParentData, parentEntity) in SystemAPI.Query<HeatGridParentData>().WithEntityAccess())
        {
            for (int i = 0; i < bufferLength; i++)
            {
                var data = buffer[i];

                if (data.chemicalMaterialID != gridParentData.gridData.chemicalMaterial.id)
                    continue;

                BatchMaterialID materialID = data.heatMapBatchMaterialID;

                foreach (var (meshInfo, child) in SystemAPI.Query<RefRW<MaterialMeshInfo>, RefRO<Parent>>())
                {
                    if (child.ValueRO.Value == parentEntity)
                    {
                        var info = meshInfo.ValueRO;
                        info.MaterialID = materialID;
                        meshInfo.ValueRW = info;
                    }
                }
            }
        }
    }

    private void SetMaterialEntity(Entity entityToApply, BatchMaterialID materialID, ref SystemState state)
    {
        var meshInfo = SystemAPI.GetComponentRW<MaterialMeshInfo>(entityToApply);

        var info = meshInfo.ValueRO;
        info.MaterialID = materialID;
        meshInfo.ValueRW = info;
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out GridData gridData))
            return;

        _heatMapEnabled = gridData.heatMapEnabled;

        if (_lastState != _heatMapEnabled)
        {
            if (_heatMapEnabled)
            {
                SetToHeatMapMaterialGlobal(ref state);
            }
            else
            {
                SetToNormalMaterialGlobal(ref state);
            }
        }

        _lastState = _heatMapEnabled;

        foreach (var (requestData, entity) in SystemAPI.Query<HeatMapUpdateRequest>().WithEntityAccess())
        {
            if (_heatMapEnabled)
            {
                SetMaterialEntity(entity, requestData.defaultHeatMapMaterialID, ref state);
            }
            else
            {
                SetMaterialEntity(entity, requestData.materialID, ref state);
            }
        }
    }
}
