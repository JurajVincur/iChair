using UnityEngine;

public class SpeedButtonsController : BleController
{
    [SerializeField] GazeSlider horizontalSlider;
    [SerializeField] Transform speedSelectTransform;
    [SerializeField] Transform controlsTransform;
    [SerializeField] int maxExtentX = 64;
    [SerializeField] int maxExtentY = 64;

    private float speed = 0.5f;
    private Vector3 defaultSpeedSelectPos;
    private Vector3 defaultControlsPos;
    private Vector3 offScreen = new Vector3(2000, 0, 0);

    private void Awake()
    {
        defaultControlsPos = controlsTransform.localPosition;
        defaultSpeedSelectPos = speedSelectTransform.localPosition;
        controlsTransform.localPosition = offScreen;
    }

    public override void Stop()
    {
        horizontalSlider.ResetState();
        controlsTransform.localPosition = offScreen;
        speedSelectTransform.localPosition = defaultSpeedSelectPos;
        speed = 0.5f;
    }

    public void SelectSpeed(float value)
    {
        speed = value;
        speedSelectTransform.localPosition = offScreen;
        controlsTransform.localPosition = defaultControlsPos;
    }

    private void Update()
    {
        ble.SetXY01(horizontalSlider.Value, speed, maxExtentX, maxExtentY);
    }
}
