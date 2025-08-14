using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public static LanguageSwitcher instance { get; private set; }

    private Language currentLanguage = Language.English;
    private List<LanguageStrings> languageStrings;

    public enum Language
    {
        None,
        English,
        Español
    }

    void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        languageStrings = new List<LanguageStrings>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            currentLanguage = (Language)PlayerPrefs.GetInt("Language");
        }
        else
        {
            currentLanguage = Language.English; // Default language
            PlayerPrefs.SetInt("Language", (int)currentLanguage);
            PlayerPrefs.Save();
        }
        UpdateLanguage();
    }


    public Language GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
        PlayerPrefs.Save();

        UpdateLanguage();
    }

    public void NewLanguageString(LanguageStrings ls)
    {
        if (!ls)
            return;

        if (languageStrings.Contains(ls))
            return;  

        languageStrings.Add(ls);
        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        for (int i = languageStrings.Count - 1; i >= 0; i--)
        {
            if (!languageStrings[i])
            {
                languageStrings.RemoveAt(i);
                continue;
            }
            languageStrings[i].UseChosenLanguageString(currentLanguage);
        }

    }
}
