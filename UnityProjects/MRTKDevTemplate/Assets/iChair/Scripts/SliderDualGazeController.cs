using MixedReality.Toolkit;
using UnityEngine;

public class SliderDualGazeController : BleController
{
    [SerializeField] GazeSlider verticalSlider;
    [SerializeField] StatefulInteractable interactableLeftSlow = null;
    [SerializeField] StatefulInteractable interactableLeftFast = null;
    [SerializeField] StatefulInteractable interactableRightSlow = null;
    [SerializeField] StatefulInteractable interactableRightFast = null;
    [SerializeField] int maxExtentX = 64;
    [SerializeField] int maxExtentY = 64;
    [SerializeField] float xSlow = 0.75f;
    [SerializeField] float xFast = 1f;

    public override void Stop()
    {
        verticalSlider.ResetState();
    }

    private void Update()
    {
        float x = 0.5f;
        if (interactableLeftFast.isHovered)
        {
            x = 1f - xFast;
        }
        else if (interactableLeftSlow.isHovered)
        {
            x = 1f - xSlow;
        }
        else if (interactableRightFast.isHovered)
        {
            x = xFast;
        }
        else if (interactableRightSlow.isHovered)
        {
            x = xSlow;
        }
        ble.SetXY01(x, verticalSlider.Value, maxExtentX, maxExtentY);
    }
}
