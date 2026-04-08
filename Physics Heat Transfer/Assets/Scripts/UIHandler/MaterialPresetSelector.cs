using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using TMPro;

public class MaterialPresetSelector : MonoBehaviour
{
    public static MaterialPresetSelector Instance;

    [SerializeField]
    private TextMeshProUGUI _materialPresetTextLabel;

    [SerializeField]
    private ChemicalMaterial _defaultMaterialPreset;

    [HideInInspector]
    public ChemicalMaterial selectedMaterialPreset;

    [SerializedDictionary(keyName: "Button", valueName: "Material Preset")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<Button, ChemicalMaterial> buttonMaterialPresetDictionary = new AYellowpaper.SerializedCollections.SerializedDictionary<Button, ChemicalMaterial>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SelectMaterialPreset(_defaultMaterialPreset);
    }

    private void Start()
    {
        foreach (Button button in buttonMaterialPresetDictionary.Keys)
        {
            button.onClick.AddListener(() => SelectMaterialPreset(buttonMaterialPresetDictionary[button]));
        }
    }

    public void SelectMaterialPreset(ChemicalMaterial materialPreset)
    {
        selectedMaterialPreset = materialPreset;
        _materialPresetTextLabel.text = $"Material: {selectedMaterialPreset.germanMaterialName}";
        Debug.Log("Changed material preset for material generation!");
    }
}