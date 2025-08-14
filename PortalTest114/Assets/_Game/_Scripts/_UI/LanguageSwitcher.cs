using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the game's language settings, allowing players to switch between languages and update UI elements accordingly.
/// </summary>
public class LanguageSwitcher : MonoBehaviour
{
    public static LanguageSwitcher instance { get; private set; }

    #region Private Variables
    private Language currentLanguage = Language.English; // Default language set to English
    private List<LanguageStrings> languageStrings; // List to hold all LanguageStrings components in the scene
    #endregion

    #region Enums
    /// <summary>
    /// The available languages for the game.
    /// </summary>
    public enum Language
    {
        None,
        English,
        Español
    }
    #endregion

    #region Unity Events
    void Awake()
    {
        // Ensure only one instance of LanguageSwitcher exists in the scene
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance to this and prevent it from being destroyed on scene load
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialise the list of LanguageStrings
        languageStrings = new List<LanguageStrings>();
    }

    private void Start()
    {
        // See if a language preference is saved in PlayerPrefs and get it, otherwise set default language
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

        // Find all LanguageStrings components in the scene and tell them the current language
        UpdateLanguage();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns the current language set in the game.
    /// </summary>
    public Language GetCurrentLanguage()
    {
        return currentLanguage;
    }

    /// <summary>
    /// Sets the current language for the game and saves it in PlayerPrefs.
    /// </summary>
    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
        PlayerPrefs.Save();

        UpdateLanguage();
    }

    /// <summary>
    /// A method to register a new LanguageStrings component to the LanguageSwitcher.
    /// </summary>
    public void NewLanguageString(LanguageStrings ls)
    {
        // If ls parameter is null, return early
        if (!ls)
            return;

        // If the languageStrings list already contains the LanguageStrings component, return early
        if (languageStrings.Contains(ls))
            return;

        // Add the new LanguageStrings component to the list and update the language
        languageStrings.Add(ls);
        UpdateLanguage();
    }

    /// <summary>
    /// Removes any null references from the languageStrings list and updates the language strings for all components.
    /// </summary>
    private void UpdateLanguage()
    {
        // Goes through each LanguageStrings component in the list and updates its language based on the current language
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
    #endregion
}
