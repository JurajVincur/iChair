using UnityEngine;
using TMPro;

public class ButtonTextChanger : MonoBehaviour
{
    public TextMeshPro buttonText;

    public void ChangeText(string newText)
    {
        buttonText.text = newText;
    }
}
