using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextChangeFileLogger : MonoBehaviour
{
    [Header("Parents that contain WriteToDisplay components")]
    [SerializeField] private Transform[] keyParents;

    [Header("Log settings")]
    [SerializeField] private string fileName = "text_log.txt";
    

    private string logPath;
    private WriteToDisplay[] keys;

    private void Awake()
    {
        logPath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);

            string sceneName = SceneManager.GetActiveScene().name;

            string separator =
                Environment.NewLine +
                "==================================================" +
                Environment.NewLine +
                $"NEW SESSION: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}" +
                Environment.NewLine +
                $"SCENE: {sceneName}" +
                Environment.NewLine +
                "==================================================" +
                Environment.NewLine;

            File.AppendAllText(logPath, separator);

            Debug.Log("Logger file path: " + logPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Logger init failed: " + ex);
        }

        keys = GetKeysFromParents();
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

    private void HandleTextChanged(string text)
    {
        try
        {
            string timestamp =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            File.AppendAllText(
                logPath,
                $"[{timestamp}] {text}{Environment.NewLine}"
            );
        }
        catch (Exception ex)
        {
            Debug.LogError("Write log failed: " + ex);
        }
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
}
