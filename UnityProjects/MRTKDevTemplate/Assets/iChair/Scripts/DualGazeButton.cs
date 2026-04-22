using MixedReality.Toolkit;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DualGazeButton : MonoBehaviour
{
    [SerializeField]
    protected float dualPopUpTime = 0.5f;
    [SerializeField]
    protected float timeToReactivate = 0.25f;
    [SerializeField]
    protected StatefulInteractable interactable = null;
    [SerializeField]
    protected StatefulInteractable confirmInteractable = null;

    protected float timer = 0f;

    protected virtual void Awake()
    {
        if (interactable == null)
        {
            interactable = GetComponent<StatefulInteractable>();
        }
        interactable.firstHoverEntered.AddListener(OnHover);
    }

    public virtual void OnHover(HoverEnterEventArgs args)
    {
        confirmInteractable.gameObject.SetActive(true);
        confirmInteractable.enabled = true;
        timer = 0f;
    }

    public virtual void OnConfirmSelectExited(SelectExitEventArgs args)
    {
        interactable.enabled = false;
        confirmInteractable.enabled = false;
        StartCoroutine(ConfirmDisableRoutine());
    }

    protected virtual IEnumerator ConfirmDisableRoutine()
    {
        yield return null;
        confirmInteractable.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(timeToReactivate);
        interactable.enabled = true;
    }

    protected virtual void Update()
    {
        if (interactable.isHovered || confirmInteractable.isActiveAndEnabled == false || confirmInteractable.isHovered)
        {
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
        if (timer >= dualPopUpTime)
        {
            confirmInteractable.gameObject.SetActive(false);
        }
    }
}
