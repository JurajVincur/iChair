using System;
using System.Linq;
using UnityEngine;

public class SpellChecker : MonoBehaviour
{
    public bool IsReady
    {
        get
        {
            return DictionaryManager.Instance != null &&
                   DictionaryManager.Instance.IsReady &&
                   DictionaryManager.Instance.SymSpell != null;
        }
    }

    private SymSpell SharedSymSpell
    {
        get
        {
            if (DictionaryManager.Instance == null)
                return null;

            return DictionaryManager.Instance.SymSpell;
        }
    }

    public string[] Suggest5(string word)
    {
        if (!IsReady) return Array.Empty<string>();
        if (string.IsNullOrWhiteSpace(word)) return Array.Empty<string>();

        var suggestions = SharedSymSpell.Lookup(word, SymSpell.Verbosity.All);

        return suggestions
            .OrderBy(s => s.distance)
            .ThenByDescending(s => s.count)
            .Take(5)
            .Select(s => s.term)
            .ToArray();
    }
}
