using UnityEngine;

public class ButtonAngleCalculator : MonoBehaviour
{
    public Transform buttonA;
    public Transform buttonB;
    public Transform keyboardTransform;

    void Start()
    {
        if (buttonA == null || buttonB == null || keyboardTransform == null)
        {
            Debug.LogError("Assign all references");
            return;
        }

        Transform a = buttonA.Find("ButtonContent") ?? buttonA;
        Transform b = buttonB.Find("ButtonContent") ?? buttonB;

        Vector3 posA = a.position;
        Vector3 posB = b.position;

        Vector3 vectorWorld = posB - posA;
        Vector3 vectorLocal = keyboardTransform.InverseTransformDirection(vectorWorld);

        float distance = vectorLocal.magnitude;
        float angle = Mathf.Atan2(vectorLocal.y, vectorLocal.x) * Mathf.Rad2Deg;

        Debug.Log("Position A: " + posA);
        Debug.Log("Position B: " + posB);
        Debug.Log("Vector (world): " + vectorWorld);
        Debug.Log("Vector (local): " + vectorLocal);
        Debug.Log("Distance: " + distance.ToString("F3"));
        Debug.Log("Raw angle: " + angle);
        Debug.Log("Formatted angle: " + angle.ToString("F2"));
    }
}
