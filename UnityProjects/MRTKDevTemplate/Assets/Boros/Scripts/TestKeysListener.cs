using UnityEngine;

public class TestKeysListener : MonoBehaviour
{
    [Header("Drag the Keys folder here")]
    [SerializeField] private Transform keysParent;

    private WriteToDisplay[] keys;

    private void Awake()
    {
        // Get all WriteToDisplay scripts inside Keys
        keys = keysParent.GetComponentsInChildren<WriteToDisplay>(true);

        Debug.Log("Found keys: " + keys.Length);
    }

    private void OnEnable()
    {
        foreach (var key in keys)
        {
            key.OnTextChanged += OnKeyPressed;
        }

        Debug.Log("Listener connected.");
    }

    private void OnDisable()
    {
        foreach (var key in keys)
        {
            key.OnTextChanged -= OnKeyPressed;
        }
    }

    private void OnKeyPressed(string text)
    {
        Debug.Log("test");
    }
}
