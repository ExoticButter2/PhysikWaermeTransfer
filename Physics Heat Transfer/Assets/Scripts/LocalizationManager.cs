using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Languages
{
    English,
    German,
    Bulgarian
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [HideInInspector]
    public Languages currentLanguage;

    public static Action<Languages> OnLanguageChange;

    [SerializeField]
    [SerializedDictionary(keyName: "Button", valueName: "Language")]
    private SerializedDictionary<Button, Languages> _buttonToLanguageDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (KeyValuePair<Button, Languages> kvp in _buttonToLanguageDictionary)
        {
            kvp.Key.onClick.AddListener(() => OnLanguageChange.Invoke(kvp.Value));
        }
    }

    private void ChangeLanguage(Languages selectedLanguage)
    {
        currentLanguage = selectedLanguage;
    }

    private void OnEnable()
    {
        OnLanguageChange += ChangeLanguage;
    }

    private void OnDisable()
    {
        OnLanguageChange -= ChangeLanguage;

        foreach (KeyValuePair<Button, Languages> kvp in _buttonToLanguageDictionary)
        {
            kvp.Key.onClick.RemoveListener(() => OnLanguageChange?.Invoke(kvp.Value));
        }
    }
}
