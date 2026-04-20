using MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.Events;

public class HoverButton : MonoBehaviour
{
    [SerializeField]
    protected StatefulInteractable interactable = null;
    [SerializeField]
    protected UnityEvent<float> onHoverStay = new UnityEvent<float>();

    protected float hoverTime = 0f;

    protected virtual void Awake()
    {
        if (interactable == null)
        {
            interactable = GetComponent<StatefulInteractable>();
        }
    }

    protected virtual void Update()
    {
        if (interactable.isHovered)
        {
            hoverTime += Time.deltaTime;
            onHoverStay.Invoke(hoverTime);
        }
        else
        {
            hoverTime = 0f;
        }
    }
}
