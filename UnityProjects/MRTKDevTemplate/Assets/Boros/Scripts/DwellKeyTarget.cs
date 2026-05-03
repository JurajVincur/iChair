using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DwellKeyTarget : MonoBehaviour
{
    public XRBaseInteractable interactable;
    public UnityEvent onDwellCompleted;

    private void Reset()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();
    }
}
