using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;

public class GridDataSettingsAuthoring : MonoBehaviour
{
    public int defaultWidth;
    public int defaultHeight;
    public int defaultDepth;
    public bool heatMapEnabled;

    public int cellSize;

    public List<ChemicalMaterial> chemicalMaterials;
    public Material heatMapMaterial;
}

public struct GridData : IComponentData
{
    public int width;
    public int height;
    public int depth;
    public bool heatMapEnabled;

    public int cellSize;

    public ChemicalMaterialBlobItem chemicalMaterial;
    public ChemicalMaterialPrefabElement chemicalMaterialPrefabElement;
}

public struct ChemicalMaterialRuntimeData : IBufferElementData
{
    public UnityObjectRef<Material> visualMaterial;
    public UnityObjectRef<Material> heatMapMaterial;
    public BatchMaterialID visualBatchMaterialID;
    public BatchMaterialID heatMapBatchMaterialID;
    public int chemicalMaterialID;
    //MAKE HEATMAP AND VISUAL MATERIAL ID ASSIGN
}

public class GridDataSettingsBaker : Baker<GridDataSettingsAuthoring>
{
    public override void Bake(GridDataSettingsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new GridData { width = authoring.defaultWidth, height = authoring.defaultHeight, depth = authoring.defaultDepth, heatMapEnabled = authoring.heatMapEnabled, cellSize = authoring.cellSize });

        var buffer = AddBuffer<ChemicalMaterialPrefabElement>(entity);
        var materialBuffer = AddBuffer<ChemicalMaterialRuntimeData>(entity);

        foreach (ChemicalMaterial chemicalMaterial in authoring.chemicalMaterials)
        {
            buffer.Add(new ChemicalMaterialPrefabElement { 
                prefabEntity = GetEntity(chemicalMaterial.prefab, TransformUsageFlags.Dynamic),
                chemicalMaterialID = chemicalMaterial.chemicalID
            });

            materialBuffer.Add(new ChemicalMaterialRuntimeData
            {
                visualMaterial = chemicalMaterial.visualMaterial,
                heatMapMaterial = chemicalMaterial.heatMapMaterial,
                visualBatchMaterialID = new BatchMaterialID(),
                heatMapBatchMaterialID = new BatchMaterialID(),
                chemicalMaterialID = chemicalMaterial.chemicalID
            });
        }
    }
}
