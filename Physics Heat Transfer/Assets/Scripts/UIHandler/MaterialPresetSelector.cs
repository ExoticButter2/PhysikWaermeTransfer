using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaterialPresetSelector : MonoBehaviour
{
    [SerializeField]
    private Material _defaultMaterialPreset;

    [HideInInspector]
    public Material _selectedMaterialPreset;

    private SerializedDictionary<Button, Material> _buttonMaterialPresetDictionary = new SerializedDictionary<Button, Material>();

    private void Awake()
    {
        _selectedMaterialPreset = _defaultMaterialPreset;
    }

    private void Start()
    {
        foreach (Button button in _buttonMaterialPresetDictionary.Keys)
        {
            button.onClick.AddListener(() => SelectMaterialPreset(_buttonMaterialPresetDictionary[button]));
        }
    }

    public void SelectMaterialPreset(Material materialPreset)
    {
        _selectedMaterialPreset = materialPreset;
    }
}