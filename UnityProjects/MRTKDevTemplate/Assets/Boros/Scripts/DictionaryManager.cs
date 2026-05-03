using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;

public class DictionaryManager : MonoBehaviour
{
    public static DictionaryManager Instance { get; private set; }

    [Header("Resources path (no extension)")]
    [SerializeField] private string dictionaryResourcePath = "frequency_dictionary_en_82_765";

    [Header("SymSpell settings")]
    [SerializeField] private int initialCapacity = 82765;
    [SerializeField] private int maxDictionaryEditDistance = 2;

    [Header("Loading")]
    [SerializeField] private int trieBuildBatchSize = 1000;

    public PrefixTrie Trie { get; private set; }
    public SymSpell SymSpell { get; private set; }
    public bool IsReady { get; private set; }

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(LoadAllCoroutine());
    }

    private IEnumerator LoadAllCoroutine()
    {
        if (isLoading || IsReady)
            yield break;

        isLoading = true;
        IsReady = false;

        TextAsset dictAsset = Resources.Load<TextAsset>(dictionaryResourcePath);
        if (dictAsset == null)
        {
            Debug.LogError($"Dictionary not found in Resources: {dictionaryResourcePath}.txt");
            isLoading = false;
            yield break;
        }

        Trie = new PrefixTrie();
        SymSpell = new SymSpell(initialCapacity, maxDictionaryEditDistance);

        // ---- Build trie in chunks so startup does not block as hard ----
        string[] lines = dictAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                continue;

            string word = parts[0].ToLowerInvariant();

            if (!long.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out long count))
                continue;

            Trie.Insert(word, count);

            if (i % trieBuildBatchSize == 0)
                yield return null;
        }

        // ---- Load SymSpell once from a real file path ----
        string dictPath = Path.Combine(Application.persistentDataPath, dictionaryResourcePath + ".txt");

        if (!File.Exists(dictPath) || new FileInfo(dictPath).Length != dictAsset.bytes.Length)
        {
            File.WriteAllBytes(dictPath, dictAsset.bytes);
        }

        bool loaded = SymSpell.LoadDictionary(dictPath, 0, 1);
        if (!loaded)
        {
            Debug.LogError("SymSpell LoadDictionary failed.");
            isLoading = false;
            yield break;
        }

        IsReady = true;
        isLoading = false;

        Debug.Log("DictionaryManager ready.");
    }
}
