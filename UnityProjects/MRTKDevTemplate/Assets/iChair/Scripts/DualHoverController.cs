using UnityEngine;

public class DualHoverController : BleController
{
    [SerializeField] Transform speedSelectTransform;
    [SerializeField] Transform controlsTransform;
    [SerializeField] int maxExtentX = 64;
    [SerializeField] int maxExtentY = 64;

    private float speed = 0.5f; //0..1
    private float x = 0.5f; //0..1
    private float speedScale = 1f;
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
        controlsTransform.localPosition = offScreen;
        speedSelectTransform.localPosition = defaultSpeedSelectPos;
        speed = 0.5f;
        SetX(0.5f);
        SetSpeedScale(1f);
    }

    public void SelectSpeed(float value)
    {
        speed = value;
        speedSelectTransform.localPosition = offScreen;
        controlsTransform.localPosition = defaultControlsPos;
    }

    public void SetX(float x)
    {
        this.x = x;
    }

    public void SetSpeedScale(float speedScale)
    {
        this.speedScale = speedScale;
    }

    private void LateUpdate()
    {
        float y = (speed - 0.5f); //-0.5..0.5
        y = y * speedScale + 0.5f;
        ble.SetXY01(x, y, maxExtentX, maxExtentY);
        SetX(0.5f);
        SetSpeedScale(1f);
    }
}

