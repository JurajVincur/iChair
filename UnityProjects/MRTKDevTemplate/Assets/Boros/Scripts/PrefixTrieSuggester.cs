using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefixTrieSuggester : MonoBehaviour
{
    [SerializeField] private int maxSuggestions = 3;

    public bool IsReady
    {
        get
        {
            return DictionaryManager.Instance != null &&
                   DictionaryManager.Instance.IsReady &&
                   DictionaryManager.Instance.Trie != null;
        }
    }

    private PrefixTrie Trie
    {
        get
        {
            if (DictionaryManager.Instance == null)
                return null;

            return DictionaryManager.Instance.Trie;
        }
    }

    public string[] Suggest5(string word)
    {
        if (!IsReady) return Array.Empty<string>();
        if (string.IsNullOrWhiteSpace(word)) return Array.Empty<string>();

        string prefix = word.ToLowerInvariant();

        return Trie.GetTopSuggestions(prefix, maxSuggestions)
            .Select(x => x.Word)
            .ToArray();
    }

    public List<NextLetterEntry> GetTop4NextLetters(string prefix)
    {
        if (!IsReady)
            return new List<NextLetterEntry>();

        if (string.IsNullOrWhiteSpace(prefix))
            return new List<NextLetterEntry>();

        prefix = prefix.ToLowerInvariant();

        var top100Words = Trie.GetTopSuggestions(prefix, 100);

        return top100Words
            .Where(x => x.Word.Length > prefix.Length)
            .GroupBy(x => x.Word[prefix.Length])
            .Select(g => new NextLetterEntry
            {
                Letter = g.Key,
                Count = g.Sum(x => x.Count)
            })
            .OrderByDescending(x => x.Count)
            .Take(4)
            .ToList();
    }

    public string[] GetTop4NextLettersAsStrings(string prefix)
    {
        if (!IsReady) return Array.Empty<string>();
        if (string.IsNullOrWhiteSpace(prefix)) return Array.Empty<string>();

        prefix = prefix.ToLowerInvariant();

        var top100Words = Trie.GetTopSuggestions(prefix, 100);

        return top100Words
            .Where(x => x.Word.Length > prefix.Length)
            .GroupBy(x => x.Word[prefix.Length])
            .Select(g => new
            {
                Letter = g.Key,
                Count = g.Sum(x => x.Count)
            })
            .OrderByDescending(x => x.Count)
            .Take(4)
            .Select(x => x.Letter.ToString())
            .ToArray();
    }
}

public class PrefixTrie
{
    private readonly TrieNode root = new TrieNode();

    public void Insert(string word, long count)
    {
        if (string.IsNullOrWhiteSpace(word))
            return;

        TrieNode node = root;

        foreach (char c in word)
        {
            if (!node.Children.TryGetValue(c, out TrieNode child))
            {
                child = new TrieNode();
                node.Children[c] = child;
            }

            node = child;
        }

        node.IsWord = true;
        node.Count = count;
    }

    public List<WordEntry> GetTopSuggestions(string prefix, int topN)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return new List<WordEntry>();

        TrieNode node = root;

        foreach (char c in prefix)
        {
            if (!node.Children.TryGetValue(c, out node))
                return new List<WordEntry>();
        }

        var results = new List<WordEntry>();
        Collect(node, prefix, results);

        return results
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToList();
    }

    private void Collect(TrieNode node, string currentWord, List<WordEntry> results)
    {
        if (node.IsWord)
        {
            results.Add(new WordEntry
            {
                Word = currentWord,
                Count = node.Count
            });
        }

        foreach (var kvp in node.Children)
        {
            Collect(kvp.Value, currentWord + kvp.Key, results);
        }
    }
}

public class TrieNode
{
    public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
    public bool IsWord;
    public long Count;
}

public class WordEntry
{
    public string Word;
    public long Count;
}

public class NextLetterEntry
{
    public char Letter;
    public long Count;
}
