using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHistoryManager : MonoBehaviour
{
    [Header("Parents that contain WriteToDisplay components")]
    [SerializeField] private Transform[] keyParents;

    [Header("Shared input field")]
    [SerializeField] private TMP_InputField inputField;

    [Header("WriteToDisplay used to type characters")]
    [SerializeField] private WriteToDisplay writerKey;

    [Header("WriteToDisplay used for backspace")]
    [SerializeField] private WriteToDisplay backspaceKey;

    private WriteToDisplay[] keys;

    private readonly List<string> history = new List<string>();
    private int historyIndex = -1;

    private bool isApplyingHistory = false;

    private void Awake()
    {
        keys = GetKeysFromParents();

        if (inputField == null)
            inputField = FindSharedInputField(keys);

        if (inputField == null)
        {
            Debug.LogError("TextHistoryManager: No TMP_InputField assigned or found.");
            enabled = false;
            return;
        }

        if (writerKey == null)
        {
            Debug.LogError("TextHistoryManager: writerKey is not assigned.");
            enabled = false;
            return;
        }

        if (backspaceKey == null)
        {
            Debug.LogError("TextHistoryManager: backspaceKey is not assigned.");
            enabled = false;
            return;
        }

        AddToHistory(inputField.text);
    }

    private void OnEnable()
    {
        if (keys == null)
            keys = GetKeysFromParents();

        foreach (var key in keys)
        {
            if (key == null) continue;
            key.OnTextChanged += HandleTextChanged;
        }
    }

    private void OnDisable()
    {
        if (keys == null) return;

        foreach (var key in keys)
        {
            if (key == null) continue;
            key.OnTextChanged -= HandleTextChanged;
        }
    }

    private void HandleTextChanged(string newText)
    {
        if (isApplyingHistory)
            return;

        if (historyIndex >= 0 && history[historyIndex] == newText)
            return;

        AddToHistory(newText);
    }

    private void AddToHistory(string text)
    {
        if (historyIndex < history.Count - 1)
        {
            history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
        }

        history.Add(text);
        historyIndex++;
    }

    public void Undo()
    {
        if (historyIndex <= 0)
            return;

        historyIndex--;
        ApplyHistoryThroughWriteToDisplay(history[historyIndex]);
    }

    public void Redo()
    {
        if (historyIndex >= history.Count - 1)
            return;

        historyIndex++;
        ApplyHistoryThroughWriteToDisplay(history[historyIndex]);
    }

    private void ApplyHistoryThroughWriteToDisplay(string targetText)
    {
        if (inputField == null)
            return;

        isApplyingHistory = true;

        string currentText = inputField.text ?? "";
        targetText ??= "";

        int commonPrefixLength = GetCommonPrefixLength(currentText, targetText);

        inputField.ActivateInputField();
        inputField.Select();
        inputField.caretPosition = currentText.Length;

        int charsToDelete = currentText.Length - commonPrefixLength;
        for (int i = 0; i < charsToDelete; i++)
        {
            backspaceKey.Backspace();
        }

        for (int i = commonPrefixLength; i < targetText.Length; i++)
        {
            writerKey.SetLetter(targetText[i].ToString());
            writerKey.Write();
        }

        isApplyingHistory = false;
    }

    private int GetCommonPrefixLength(string a, string b)
    {
        int max = Mathf.Min(a.Length, b.Length);
        int i = 0;

        while (i < max && a[i] == b[i])
            i++;

        return i;
    }

    private WriteToDisplay[] GetKeysFromParents()
    {
        var allKeys = new List<WriteToDisplay>();

        if (keyParents == null)
            return allKeys.ToArray();

        foreach (var parent in keyParents)
        {
            if (parent == null) continue;
            allKeys.AddRange(parent.GetComponentsInChildren<WriteToDisplay>(true));
        }

        return allKeys.ToArray();
    }

    private TMP_InputField FindSharedInputField(WriteToDisplay[] writeToDisplays)
    {
        if (writeToDisplays == null)
            return null;

        foreach (var key in writeToDisplays)
        {
            if (key != null && key.inputField != null)
                return key.inputField;
        }

        return null;
    }
}
