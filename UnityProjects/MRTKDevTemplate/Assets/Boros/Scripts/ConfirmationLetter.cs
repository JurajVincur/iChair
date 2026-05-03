using UnityEngine;
using TMPro;

public class ConfirmationLetter : MonoBehaviour
{
    public enum PlacementDirection
    {
        Down,
        Up,
        Left,
        Right
    }

    [Header("Confirmation Button Label")]
    [SerializeField] private TMP_Text confirmationLabel;

    [Header("Write To Display on this confirmation button")]
    [SerializeField] private WriteToDisplay writeToDisplay;

    [Header("Position Settings")]
    [SerializeField] private PlacementDirection placementDirection = PlacementDirection.Down;

    [SerializeField] private float offsetDistance = 0.03f;

    private void Awake()
    {
        if (writeToDisplay == null)
        {
            writeToDisplay = GetComponent<WriteToDisplay>();
        }
    }

    public void MoveLetter(Transform caller, string labelText)
    {
        if (caller == null)
        {
            Debug.LogWarning("MoveLetter called with null caller.");
            return;
        }

        // Move in selected direction
        transform.position = caller.position + GetDirectionVector(caller) * offsetDistance;

        // Match rotation
        transform.rotation = caller.rotation;

        // Copy label text to confirmation button
        if (confirmationLabel != null)
        {
            confirmationLabel.text = labelText;
        }

        // Set letter for WriteToDisplay
        if (writeToDisplay != null)
        {
            writeToDisplay.SetLetter(labelText);
        }

        // Show confirmation button
        gameObject.SetActive(true);
    }

    public void HideLetter()
    {
        gameObject.SetActive(false);
    }

    private Vector3 GetDirectionVector(Transform caller)
    {
        switch (placementDirection)
        {
            case PlacementDirection.Up:
                return caller.up;

            case PlacementDirection.Down:
                return -caller.up;

            case PlacementDirection.Left:
                return -caller.right;

            case PlacementDirection.Right:
                return caller.right;

            default:
                return -caller.up;
        }
    }
}
