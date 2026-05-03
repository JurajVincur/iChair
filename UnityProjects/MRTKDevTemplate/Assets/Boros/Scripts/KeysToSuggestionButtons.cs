using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeysToSuggestionButtons : MonoBehaviour
{
    [Header("Keys folders")]
    [SerializeField] private Transform[] keyParents;

    [Header("Mode")]
    [SerializeField] private int mode = 1;
    // 1 = old SpellChecker words
    // 2 = PrefixTrie top 4 next letters

    [Header("Suggesters")]
    [SerializeField] private SpellChecker spellChecker;
    [SerializeField] private PrefixTrieSuggester prefixTrieSuggester;

    [Header("Suggestion label objects")]
    [SerializeField] private TMP_Text[] labels;

    [Header("Assign manually in Inspector in fixed order")]
    [SerializeField] private WriteToDisplay[] suggestionKeys;

    private WriteToDisplay[] keys;

    private void Awake()
    {
        keys = GetKeysFromParents();

        ClearLabels();
        ApplyModeToSuggestionKeys();
    }

    private WriteToDisplay[] GetKeysFromParents()
    {
        var allKeys = new List<WriteToDisplay>();

        if (keyParents == null) return allKeys.ToArray();

        foreach (var parent in keyParents)
        {
            if (parent == null) continue;
            allKeys.AddRange(parent.GetComponentsInChildren<WriteToDisplay>(true));
        }

        return allKeys.ToArray();
    }

    private void OnEnable()
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                if (key != null)
                    key.OnTextChanged += OnTextChanged;
            }
        }

        if (suggestionKeys != null)
        {
            foreach (var key in suggestionKeys)
            {
                if (key != null)
                    key.OnTextChanged += OnTextChanged;
            }
        }
    }

    private void OnDisable()
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                if (key != null)
                    key.OnTextChanged -= OnTextChanged;
            }
        }

        if (suggestionKeys != null)
        {
            foreach (var key in suggestionKeys)
            {
                if (key != null)
                    key.OnTextChanged -= OnTextChanged;
            }
        }
    }

    private void ApplyModeToSuggestionKeys()
    {
        if (suggestionKeys == null) return;

        foreach (var key in suggestionKeys)
        {
            if (key != null)
                key.mode = mode;
        }
    }

    private void OnTextChanged(string text)
    {
        if (!CurrentModeReady())
        {
            ClearLabels();
            Debug.Log("Suggester not ready");
            return;
        }

        string lastWord = GetLastWord(text).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(lastWord))
        {
            ClearLabels();
            return;
        }

        var suggestions = GetSuggestions(lastWord);

        for (int i = 0; i < labels.Length; i++)
        {
            bool has = suggestions != null &&
                       suggestions.Length > i &&
                       !string.IsNullOrWhiteSpace(suggestions[i]);

            if (labels[i] != null)
            {
                labels[i].text = has ? suggestions[i].ToUpper() : "";
                labels[i].gameObject.SetActive(has);
            }

            if (suggestionKeys != null &&
                suggestionKeys.Length > i &&
                suggestionKeys[i] != null)
            {
                suggestionKeys[i].SetLetter(has ? suggestions[i] : "");
            }
        }
    }

    private bool CurrentModeReady()
    {
        if (mode == 1)
            return spellChecker != null && spellChecker.IsReady;

        if (mode == 2)
            return prefixTrieSuggester != null && prefixTrieSuggester.IsReady;

        return false;
    }

    private string[] GetSuggestions(string lastWord)
    {
        if (mode == 1)
            return spellChecker != null ? spellChecker.Suggest5(lastWord) : System.Array.Empty<string>();

        if (mode == 2)
            return prefixTrieSuggester != null ? prefixTrieSuggester.GetTop4NextLettersAsStrings(lastWord) : System.Array.Empty<string>();

        return System.Array.Empty<string>();
    }

    private void ClearLabels()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] != null)
            {
                labels[i].text = "";
                labels[i].gameObject.SetActive(false);
            }

            if (suggestionKeys != null &&
                suggestionKeys.Length > i &&
                suggestionKeys[i] != null)
            {
                suggestionKeys[i].SetLetter("");
            }
        }
    }

    private static string GetLastWord(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";

        s = s.Replace('\u00A0', ' ').Trim();

        int lastSpace = s.LastIndexOf(' ');
        if (lastSpace < 0) return s;

        return s.Substring(lastSpace + 1);
    }
}
