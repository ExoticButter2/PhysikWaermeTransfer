using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public class ChemicalMaterialLibraryAuthoring : MonoBehaviour
{
    public List<ChemicalMaterial> chemicalMaterials;
    [SerializedDictionary(keyName: "Chemical material", valueName: "Material")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<ChemicalMaterial, Material> chemicalToMaterialDictionary;
}

public struct ChemicalMaterialPrefabElement : IBufferElementData
{
    public Entity prefabEntity;
    public int chemicalMaterialID;
}

public class ChemicalMaterialLibraryBaker : Baker<ChemicalMaterialLibraryAuthoring>
{
    public override void Bake(ChemicalMaterialLibraryAuthoring authoring)
    {
        using BlobBuilder builder = new BlobBuilder(Allocator.Temp);

        ref ChemicalMaterialBlob root = ref builder.ConstructRoot<ChemicalMaterialBlob>();

        int libraryLength = authoring.chemicalMaterials.Count;
        BlobBuilderArray<ChemicalMaterialBlobItem> arrayBuilder = builder.Allocate(ref root.Chemicals, libraryLength);

        for (int i = 0; i < libraryLength; i++)
        {
            arrayBuilder[i] = new ChemicalMaterialBlobItem
            {
                id = i,
                englishMaterialName = authoring.chemicalMaterials[i].englishMaterialName,
                germanMaterialName = authoring.chemicalMaterials[i].germanMaterialName,
                thermalConductivity = authoring.chemicalMaterials[i].thermalConductivity,
                densityPerCubicCm = authoring.chemicalMaterials[i].densityPerCubicCm,
                specificHeat = authoring.chemicalMaterials[i].specificHeat,
            };
        }

        var blobRef = builder.CreateBlobAssetReference<ChemicalMaterialBlob>(Allocator.Persistent);

        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new ChemicalMaterialBlobReference { Value = blobRef });

        AddBlobAsset(ref blobRef, out _);

        Material[] materialArray = new Material[libraryLength];

        for (int i = 0; i < libraryLength; i++)
        {
            materialArray[i] = authoring.chemicalMaterials[i].visualMaterial;
        }
    }
}
