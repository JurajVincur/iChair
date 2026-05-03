using TMPro;
using UnityEngine;

public class TestListener : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;

    private void Awake()
    {
        Debug.Log("TestListener Awake");

        if (input == null)
        {
            Debug.LogError("TMP_InputField reference is NOT assigned in the Inspector.");
            return;
        }

        input.onValueChanged.AddListener(OnTextChanged);
        Debug.Log("Listener added.");
    }

    private void OnDestroy()
    {
        if (input != null)
            input.onValueChanged.RemoveListener(OnTextChanged);
    }

    private void OnTextChanged(string value)
    {
        Debug.Log("test");
    }
}
