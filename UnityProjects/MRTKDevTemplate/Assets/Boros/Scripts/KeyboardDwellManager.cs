using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyboardDwellManager : MonoBehaviour
{
    [SerializeField] private float dwellTime = 0.6f;
    [SerializeField] private List<DwellKeyTarget> keys = new();

    private Coroutine activeDwellCoroutine;
    private DwellKeyTarget currentKey;

    private void OnEnable()
    {
        foreach (var key in keys)
        {
            if (key != null && key.interactable != null)
            {
                key.interactable.hoverEntered.AddListener(OnHoverEntered);
                key.interactable.hoverExited.AddListener(OnHoverExited);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var key in keys)
        {
            if (key != null && key.interactable != null)
            {
                key.interactable.hoverEntered.RemoveListener(OnHoverEntered);
                key.interactable.hoverExited.RemoveListener(OnHoverExited);
            }
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        DwellKeyTarget hoveredKey = FindKey(args.interactableObject as XRBaseInteractable);
        if (hoveredKey == null)
            return;

        CancelCurrentDwell();

        currentKey = hoveredKey;
        activeDwellCoroutine = StartCoroutine(DwellTimer(hoveredKey));
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        XRBaseInteractable exitedInteractable = args.interactableObject as XRBaseInteractable;

        if (currentKey != null && currentKey.interactable == exitedInteractable)
        {
            CancelCurrentDwell();
        }
    }

    private IEnumerator DwellTimer(DwellKeyTarget key)
    {
        yield return new WaitForSeconds(dwellTime);

        if (currentKey == key)
        {
            key.onDwellCompleted?.Invoke();
        }

        activeDwellCoroutine = null;
        currentKey = null;
    }

    private void CancelCurrentDwell()
    {
        if (activeDwellCoroutine != null)
        {
            StopCoroutine(activeDwellCoroutine);
            activeDwellCoroutine = null;
        }

        currentKey = null;
    }

    private DwellKeyTarget FindKey(XRBaseInteractable interactable)
    {
        foreach (var key in keys)
        {
            if (key != null && key.interactable == interactable)
                return key;
        }

        return null;
    }
}
