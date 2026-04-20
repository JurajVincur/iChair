using UnityEngine;

public class DualSliderController : BleController
{
    [SerializeField] GazeSlider horizontalSlider;
    [SerializeField] GazeSlider verticalSlider;
    [SerializeField] int maxExtentX = 64;
    [SerializeField] int maxExtentY = 64;

    public override void Stop()
    {
        horizontalSlider.ResetState();
        verticalSlider.ResetState();
    }

    private void Update()
    {
        ble.SetXY01(horizontalSlider.Value, verticalSlider.Value, maxExtentX, maxExtentY);
    }
}
