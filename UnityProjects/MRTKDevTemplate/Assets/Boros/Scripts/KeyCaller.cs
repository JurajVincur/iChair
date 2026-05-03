using UnityEngine;
using TMPro;

public class KeyCaller : MonoBehaviour
{
    [Header("Reference to the confirmation button script")]
    [SerializeField] private ConfirmationLetter confirmationLetter;

    private TMP_Text keyLabel;

    private void Awake()
    {
        keyLabel = FindKeyLabel();
    }

    public void CallMoveLetter()
    {
        if (confirmationLetter == null)
        {
            Debug.LogWarning($"ConfirmationLetter is not assigned on {gameObject.name}.");
            return;
        }

        if (keyLabel == null)
        {
            Debug.LogWarning($"No TMP label was found on {gameObject.name}.");
            return;
        }

        confirmationLetter.MoveLetter(transform, keyLabel.text);
    }

    private TMP_Text FindKeyLabel()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        if (texts.Length == 0)
            return null;

        if (texts.Length == 1)
            return texts[0];

        foreach (TMP_Text text in texts)
        {
            if (text != null && !string.IsNullOrWhiteSpace(text.text))
                return text;
        }

        return texts[0];
    }
}
