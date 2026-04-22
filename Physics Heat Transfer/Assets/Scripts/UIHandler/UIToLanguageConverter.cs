using TMPro;
using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

public class UIToLanguageConverter : MonoBehaviour
{
    #region INSTANCES
    private LocalizationManager _localizationManager;
    private MaterialPresetSelector _materialPresetSelector;
    private SizeSliderHandler _sizeSliderHandler;
    private TemperatureUnitSelector _temperatureUnitSelector;
    #endregion

    [SerializeField]
    private TextMeshProUGUI _generateButtonLabel;
    [SerializeField]
    private TextMeshProUGUI _clearButtonLabel;
    [SerializeField]
    private TextMeshProUGUI _materialSelectorLabel;
    [SerializeField]
    private TextMeshProUGUI _unitSelectorLabel;
    [SerializeField]
    private TextMeshProUGUI _mouseClickHeaterLabel;
    [SerializeField]
    private TextMeshProUGUI _languageToggleButtonLabel;

    [SerializeField]
    private List<TextMeshProUGUI> _XYZSizeModifierLabels;//width/height/depth
    [SerializeField]
    private List<TextMeshProUGUI> _KCFLabelsList;//kelvin/celsius/fahrenheit

    [SerializeField]
    [SerializedDictionary(keyName: "Text Label", valueName: "Chemical Material")]
    private SerializedDictionary<TextMeshProUGUI, ChemicalMaterial> _labelToMaterialDictionary;

    private void UpdateXYZModifiersToLanguage(Languages language)
    {
        switch (language)
        {
            case Languages.English:
                _XYZSizeModifierLabels[0].text = $"Width: {_sizeSliderHandler.width}";
                _XYZSizeModifierLabels[1].text = $"Height: {_sizeSliderHandler.height}";
                _XYZSizeModifierLabels[2].text = $"Depth: {_sizeSliderHandler.depth}";
                break;

            case Languages.German:
                _XYZSizeModifierLabels[0].text = $"Breite: {_sizeSliderHandler.width}";
                _XYZSizeModifierLabels[1].text = $"Höhe: {_sizeSliderHandler.height}";
                _XYZSizeModifierLabels[2].text = $"Tiefe: {_sizeSliderHandler.depth}";
                break;

            case Languages.Bulgarian:
                _XYZSizeModifierLabels[0].text = $"Ширина: {_sizeSliderHandler.width}";
                _XYZSizeModifierLabels[1].text = $"Височина: {_sizeSliderHandler.height}";
                _XYZSizeModifierLabels[2].text = $"Дълбочина: {_sizeSliderHandler.depth}";
                break;
        }
    }

    private void UpdateMaterialLabelsToLanguage(Languages language)
    {
        foreach (var kvp in _labelToMaterialDictionary)
        {
            switch (language)
            {
                case Languages.English:
                    kvp.Key.text = kvp.Value.englishMaterialName;
                    break;

                case Languages.German:
                    kvp.Key.text = kvp.Value.germanMaterialName;
                    break;

                case Languages.Bulgarian:
                    kvp.Key.text = kvp.Value.bulgarianMaterialName;
                    break;
            }
        }
    }

    private void UpdateTemperatureUnitsToLanguage(Languages language)
    {
        switch (language)
        {
            case Languages.English:
                _KCFLabelsList[0].text = "Kelvin";
                _KCFLabelsList[1].text = "Celsius";
                _KCFLabelsList[2].text = "Fahrenheit";
                break;

            case Languages.German:
                _KCFLabelsList[0].text = "Kelvin";
                _KCFLabelsList[1].text = "Celsius";
                _KCFLabelsList[2].text = "Fahrenheit";
                break;

            case Languages.Bulgarian:
                _KCFLabelsList[0].text = "Келвин";
                _KCFLabelsList[1].text = "Целзий";
                _KCFLabelsList[2].text = "Фаренхайт";
                break;
        }
    }

    private void UpdateUIToLanguage(Languages language)
    {
        UpdateXYZModifiersToLanguage(language);
        UpdateMaterialLabelsToLanguage(language);
        UpdateTemperatureUnitsToLanguage(language);

        switch (language)
        {
            case Languages.English:
                _generateButtonLabel.text = "Generate";
                _clearButtonLabel.text = "RESET";

                if (_materialPresetSelector.BlobReference.Value.IsCreated)
                {
                    _materialSelectorLabel.text = $"Material: {_materialPresetSelector.SelectedMaterialPreset.englishMaterialName}";
                }
                else
                {
                    _materialSelectorLabel.text = $"Material: {_materialPresetSelector.DefaultMaterial.englishMaterialName}";
                }

                _unitSelectorLabel.text = $"Measurement unit: {_temperatureUnitSelector.selectedTemperatureUnit}";
                _mouseClickHeaterLabel.text = "K/s for LMB";
                _languageToggleButtonLabel.text = "Languages";
                break;

            case Languages.German:
                _generateButtonLabel.text = "Generieren";
                _clearButtonLabel.text = "Zurücksetzen";

                if (_materialPresetSelector.BlobReference.Value.IsCreated)
                {
                    _materialSelectorLabel.text = $"Material: {_materialPresetSelector.SelectedMaterialPreset.germanMaterialName}";
                }
                else
                {
                    _materialSelectorLabel.text = $"Material: {_materialPresetSelector.DefaultMaterial.germanMaterialName}";
                }

                _unitSelectorLabel.text = $"Maßeinheit: {_temperatureUnitSelector.selectedTemperatureUnit}";
                _mouseClickHeaterLabel.text = "K/s für LMB";
                _languageToggleButtonLabel.text = "Sprachen";
                break;

            case Languages.Bulgarian:
                _generateButtonLabel.text = "Генериране";
                _clearButtonLabel.text = "Нулиране";

                if (_materialPresetSelector.BlobReference.Value.IsCreated)
                {
                    _materialSelectorLabel.text = $"Материал: {_materialPresetSelector.SelectedMaterialPreset.bulgarianMaterialName}";
                }
                else
                {
                    _materialSelectorLabel.text = $"Материал: {_materialPresetSelector.DefaultMaterial.bulgarianMaterialName}";
                }

                _unitSelectorLabel.text = $"Мерна единица: {_temperatureUnitSelector.selectedTemperatureUnit}";
                _mouseClickHeaterLabel.text = "K/s с LMB";
                _languageToggleButtonLabel.text = "Езици";
                break;
        }
    }

    private void Start()
    {
        _localizationManager = LocalizationManager.Instance;
        _materialPresetSelector = MaterialPresetSelector.Instance;
        _sizeSliderHandler = SizeSliderHandler.Instance;
        _temperatureUnitSelector = TemperatureUnitSelector.Instance;

        UpdateUIToLanguage(_localizationManager.currentLanguage);
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChange += UpdateUIToLanguage;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChange -= UpdateUIToLanguage;
    }
}
