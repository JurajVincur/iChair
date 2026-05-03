using UnityEngine;
using TMPro;
using System;

public class WriteToDisplay : MonoBehaviour
{
    [Header("»o sa m· zapÌsaù")]
    public string letter = "A";

    [Header("Kam sa m· zapÌsaù (TMP Input Field)")]
    public TMP_InputField inputField;

    public event Action<string> OnTextChanged;

    public int mode = 2;
    public bool addSpaceAfter = true;

    public void Write()
    {
        if (inputField == null)
        {
            Debug.LogWarning("InputField nie je priraden˝!");
            return;
        }

        if (string.IsNullOrEmpty(letter))
            return;

        inputField.ActivateInputField();
        inputField.Select();

        string current = inputField.text;
        int pos = inputField.caretPosition;

        if (mode == 1)
        {
            string updated = ReplaceLastWord(current, letter, addSpaceAfter).ToLowerInvariant();
            inputField.text = updated;
            inputField.caretPosition = inputField.text.Length;
        }
        else if (mode == 2)
        {
            string toInsert = letter.ToLowerInvariant();
            string updated = current.Insert(pos, toInsert);
            inputField.text = updated;
            inputField.caretPosition = pos + toInsert.Length;
        }

        NotifyChanged();
    }

    public void Backspace()
    {
        if (inputField == null)
        {
            Debug.LogWarning("InputField nie je priraden˝!");
            return;
        }

        inputField.ActivateInputField();
        inputField.Select();

        string current = inputField.text;
        int pos = inputField.caretPosition;

        if (current.Length > 0 && pos > 0)
        {
            inputField.text = current.Remove(pos - 1, 1);
            inputField.caretPosition = pos - 1;
            Log(inputField.text);
            NotifyChanged();
        }
    }

    public void Clear()
    {
        if (inputField == null)
        {
            Debug.LogWarning("InputField nie je priraden˝!");
            return;
        }

        inputField.text = "";
        inputField.caretPosition = 0;
        inputField.ActivateInputField();
        inputField.Select();

        Log(inputField.text);
        NotifyChanged();
    }

    public void SetLetter(string newLetter)
    {
        letter = newLetter;
    }

    private void Log(string finalText)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Debug.Log($"[Logger {timestamp}] {finalText}");
    }

    private void NotifyChanged()
    {
        string text = inputField != null ? inputField.text : "";
        OnTextChanged?.Invoke(text);
    }

    private static string ReplaceLastWord(string original, string replacement, bool addSpace)
    {
        if (original == null) original = "";

        original = original.Replace('\u00A0', ' ');

        int end = original.Length - 1;
        while (end >= 0 && char.IsWhiteSpace(original[end])) end--;

        if (end < 0)
            return addSpace ? (replacement + " ") : replacement;

        int start = end;
        while (start >= 0 && !char.IsWhiteSpace(original[start])) start--;

        string prefix = original.Substring(0, start + 1);

        string result = prefix + replacement;
        if (addSpace) result += " ";

        return result;
    }
}
