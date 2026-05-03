using System.Collections.Generic;
using UnityEngine;

public class KeyInteractivityManager : MonoBehaviour
{
    [Header("Assign all 3rd-layer key roots (GameObjects)")]
    public List<GameObject> keyObjects = new();

    private bool keysLocked = false;

    // Call from each key's On Hover Entered
    public void OnKeyHoverEnter()
    {
        if (keysLocked) return;
        keysLocked = true;

        foreach (var go in keyObjects)
        {
            if (!go) continue;
            foreach (var col in go.GetComponentsInChildren<Collider>(includeInactive: true))
                col.enabled = false;   // visible but not interactable
        }
    }

    // Call from RESET button
    public void ResetKeys()
    {
        keysLocked = false;

        foreach (var go in keyObjects)
        {
            if (!go) continue;
            foreach (var col in go.GetComponentsInChildren<Collider>(includeInactive: true))
                col.enabled = true;    // re-enable interaction
        }
    }
}
