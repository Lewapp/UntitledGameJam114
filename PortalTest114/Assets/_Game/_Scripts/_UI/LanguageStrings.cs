using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Deals with the strings for different languages in the game.
/// Should be attached to a GameObject with a TextMeshProUGUI component or has a child with the componenent.
/// </summary>
public class LanguageStrings : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private TextMeshProUGUI textMeshProComponent; // Reference to the TextMeshProUGUI component to display the language string
    [SerializeField] private List<LanguageInfo> languageStrings; // List of language strings for different languages
    #endregion

    #region Unity Events
    private void Start()
    {
        // Notify the LanguageSwitcher that a new LanguageStrings instance has been created
        LanguageSwitcher.instance?.NewLanguageString(this);
    }

    private void OnValidate()
    {
        // If the languageStrings list is null, do nothing
        if (languageStrings == null) 
            return;

        // Keep only the first occurrence of each language
        var seen = new HashSet<LanguageSwitcher.Language>();

        // Remove duplicates based on the language field
        for (int i = languageStrings.Count - 1; i >= 0; i--)
        {
            if (seen.Contains(languageStrings[i].language))
            {
                languageStrings[i].language = LanguageSwitcher.Language.None;
            }
            else
            {
                seen.Add(languageStrings[i].language);
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Goes through the languageStrings list and sets the text of the TextMeshProUGUI component to the string
    /// </summary>
    public void UseChosenLanguageString(LanguageSwitcher.Language language)
    {
        foreach (var langInfo in languageStrings)
        {
            // If the language matches, set the text of the TextMeshProUGUI component
            if (langInfo.language == language)
            {
                textMeshProComponent.text = langInfo.text;
                return;
            }
        }
    }
    #endregion

    #region Sub Classes
    /// <summary>
    /// Pairs the language with the text to display in the TextMeshProUGUI component.
    /// Allows for a larger text area to accommodate longer strings in different languages.
    /// Dictionary would be better, but the larger text area is more convenient for editing in the inspector.
    /// </summary>
    [Serializable]
    public class LanguageInfo
    {
        public LanguageSwitcher.Language language;

        [TextArea(3, 10)]
        public string text;
    }
    #endregion
}
