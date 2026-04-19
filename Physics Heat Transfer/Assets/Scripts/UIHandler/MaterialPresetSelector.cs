using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using TMPro;
using Unity.Entities;
using System.Collections;

public class MaterialPresetSelector : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _materialPresetTextLabel;

    private ChemicalMaterialBlobItem _selectedMaterialPreset;

    [SerializedDictionary(keyName: "Button", valueName: "ID")]
    public SerializedDictionary<Button, int> buttonToIdDictionary = new SerializedDictionary<Button, int>();

    [SerializeField]
    private int _defaultMaterialId = 0;

    private IEnumerator Start()
    {
        foreach (Button button in buttonToIdDictionary.Keys)
        {
            button.onClick.AddListener(() => SelectMaterialPreset(button));
        }

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = entityManager.CreateEntityQuery(typeof(ChemicalMaterialBlobReference));

        while (query.IsEmpty)
        {
            yield return null;
        }

        SetToDefaultMaterial(entityManager, query);
        UpdateGridMaterialSetting(entityManager);
    }

    //sets variables of this monobehaviour (in this method selectedMaterialPreset)
    #region MONO_SETTERS
    private void SetToDefaultMaterial(EntityManager entityManager, EntityQuery query)
    {
        Entity materialLibraryEntity = query.GetSingletonEntity();

        ChemicalMaterialBlobReference blobReference = entityManager.GetComponentData<ChemicalMaterialBlobReference>(materialLibraryEntity);

        _selectedMaterialPreset = blobReference.Value.Value.Chemicals[_defaultMaterialId];
    }

    private void UpdateMaterialPreset(EntityManager entityManager, Button button)
    {
        Entity materialLibraryEntity = entityManager.CreateEntityQuery(typeof(ChemicalMaterialBlobReference)).GetSingletonEntity();

        ChemicalMaterialBlobReference blobReference = entityManager.GetComponentData<ChemicalMaterialBlobReference>(materialLibraryEntity);

        int id = buttonToIdDictionary[button];

        _selectedMaterialPreset = blobReference.Value.Value.Chemicals[id];
    }
    #endregion

    //sets variables of global entity (in this method gridsettings)
    #region ENTITY_SETTERS
    private void UpdateGridMaterialSetting(EntityManager entityManager)
    {
        Entity gridSettingsEntity = entityManager.CreateEntityQuery(typeof(GridData)).GetSingletonEntity();
        GridData gridSettings = entityManager.GetComponentData<GridData>(gridSettingsEntity);
        gridSettings.chemicalMaterial = _selectedMaterialPreset;

        entityManager.SetComponentData(gridSettingsEntity, gridSettings);
    }
    #endregion

    public void SelectMaterialPreset(Button button)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        UpdateMaterialPreset(entityManager, button);

        UpdateGridMaterialSetting(entityManager);

        _materialPresetTextLabel.text = $"Material: {_selectedMaterialPreset.germanMaterialName}";
        Debug.Log("Changed material preset for material generation!");
    }
}