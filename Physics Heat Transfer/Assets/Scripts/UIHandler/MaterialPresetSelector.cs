using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using TMPro;
using Unity.Entities;
using System.Collections;

public class MaterialPresetSelector : MonoBehaviour
{
    private LocalizationManager _localizationManager;
    public static MaterialPresetSelector Instance;

    [SerializeField]
    private TextMeshProUGUI _materialPresetTextLabel;

    [HideInInspector]
    public ChemicalMaterialBlobItem SelectedMaterialPreset;
    [HideInInspector]
    public ChemicalMaterialBlobReference BlobReference;

    [SerializedDictionary(keyName: "Button", valueName: "ID")]
    public SerializedDictionary<Button, int> buttonToIdDictionary = new SerializedDictionary<Button, int>();

    [SerializeField]
    private int _defaultMaterialId = 0;

    public ChemicalMaterial DefaultMaterial;

    private void Awake()
    {
        _localizationManager = LocalizationManager.Instance;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        SelectedMaterialPreset = blobReference.Value.Value.Chemicals[_defaultMaterialId];
    }

    private void UpdateMaterialPreset(EntityManager entityManager, Button button)
    {
        Entity materialLibraryEntity = entityManager.CreateEntityQuery(typeof(ChemicalMaterialBlobReference)).GetSingletonEntity();

        ChemicalMaterialBlobReference blobReference = entityManager.GetComponentData<ChemicalMaterialBlobReference>(materialLibraryEntity);
        BlobReference = blobReference;

        int id = buttonToIdDictionary[button];

        SelectedMaterialPreset = blobReference.Value.Value.Chemicals[id];
    }
    #endregion

    //sets variables of global entity (in this method gridsettings)
    #region ENTITY_SETTERS
    private void UpdateGridMaterialSetting(EntityManager entityManager)
    {
        Entity gridSettingsEntity = entityManager.CreateEntityQuery(typeof(GridData)).GetSingletonEntity();
        GridData gridSettings = entityManager.GetComponentData<GridData>(gridSettingsEntity);
        gridSettings.chemicalMaterial = SelectedMaterialPreset;

        entityManager.SetComponentData(gridSettingsEntity, gridSettings);
    }
    #endregion

    public void SelectMaterialPreset(Button button)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        UpdateMaterialPreset(entityManager, button);

        UpdateGridMaterialSetting(entityManager);

        switch (_localizationManager.currentLanguage)
        {
            case Languages.English:
                _materialPresetTextLabel.text = $"Material: {SelectedMaterialPreset.englishMaterialName}";
                break;

            case Languages.German:
                _materialPresetTextLabel.text = $"Material: {SelectedMaterialPreset.germanMaterialName}";
                break;

            case Languages.Bulgarian:
                _materialPresetTextLabel.text = $"Материал: {SelectedMaterialPreset.bulgarianMaterialName}";
                break;
        }
        Debug.Log("Changed material preset for material generation!");
    }
}