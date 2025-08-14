using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageStrings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProComponent;
    [SerializeField] private List<LanguageInfo> languageStrings;

    private void Start()
    {
        LanguageSwitcher.instance?.NewLanguageString(this);
    }

    private void OnValidate()
    {
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

    public void UseChosenLanguageString(LanguageSwitcher.Language language)
    {
        foreach (var langInfo in languageStrings)
        {
            if (langInfo.language == language)
            {
                textMeshProComponent.text = langInfo.text;
                return;
            }
        }
    }

    [Serializable]
    public class LanguageInfo
    {
        public LanguageSwitcher.Language language;

        [TextArea(3, 10)]
        public string text;
    }
}
