using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LanguageSettingsToggler : MonoBehaviour
{
    [SerializeField]
    private Button _languageSettingsToggleButton;

    [SerializeField]
    private GameObject _languageScrollViewGameObject;

    public void ToggleLanguageSettingsUI()
    {
        _languageScrollViewGameObject.SetActive(!_languageScrollViewGameObject.activeInHierarchy);
    }
}
