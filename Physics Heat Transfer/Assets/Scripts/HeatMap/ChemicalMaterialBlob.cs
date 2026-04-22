using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ChemicalMaterialBlobItem
{
    public int id;
    public FixedString64Bytes englishMaterialName;
    public FixedString64Bytes germanMaterialName;
    public FixedString64Bytes bulgarianMaterialName;

    public float thermalConductivity;//in W/(cm*K)
    public float densityPerCubicCm;//grams per cubic centimeter
    public float specificHeat;//joules per gramm for 1 degree increase in temperature
}

public struct ChemicalMaterialBlob
{
    public BlobArray<ChemicalMaterialBlobItem> Chemicals;
}

public struct ChemicalMaterialBlobReference : IComponentData
{
    public BlobAssetReference<ChemicalMaterialBlob> Value;
}
