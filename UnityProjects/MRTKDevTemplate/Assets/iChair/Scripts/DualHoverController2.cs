using UnityEngine;

public class DualHoverController2 : BleController
{
    [SerializeField] Transform speedSelectTransform;
    [SerializeField] Transform controlsTransform;
    [SerializeField] float deactivationDelay = 0.2f;
    [SerializeField] Vector4[] speedSettings;

    private int turnDir = -1; //-1 or 0 or 1
    private bool turnInPlace = false;
    private int speedLevel = 2;
    private Vector3 defaultSpeedSelectPos;
    private Vector3 defaultControlsPos;
    private Vector3 offScreen = new Vector3(2000, 0, 0);
    private float deactivationTimerX = 0f;
    private float deactivationTimerTurbo = 0f;
    private bool turbo = false;

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
        SelectSpeedLevel(2, false);
        Turn(0, false);
        turbo = false;
    }

    public void SelectSpeedLevel(int level)
    {
        SelectSpeedLevel(level, true);
    }

    public void SelectSpeedLevel(int level, bool hide) //0,1,2,3...
    {
        speedLevel = level;
        if (hide)
        {
            speedSelectTransform.localPosition = offScreen;
            controlsTransform.localPosition = defaultControlsPos;
        }
    }

    public void TurnLeft(bool inPlace)
    {
        Turn(-1, inPlace);
    }

    public void TurnRight(bool inPlace)
    {
        Turn(1, inPlace);
    }

    private void Turn(int direction, bool inPlace)
    {
        turnDir = direction;
        turnInPlace = inPlace;
        deactivationTimerX = 0f;
    }

    public void EnableTurbo()
    {
        turbo = true;
        deactivationTimerTurbo = 0f;
    }

    private void LateUpdate()
    {
        Vector4 settings = speedSettings[speedLevel];
        int y = turnInPlace ? 128 : 128 + (int)(turbo ? settings.y : settings.x);
        int x = 128 + (int)(turnInPlace ? settings.w : settings.z) * turnDir;
        ble.SetXY(x, y);
        deactivationTimerX += Time.deltaTime;
        if (deactivationTimerX > deactivationDelay)
        {
            Turn(0, false);
        }
        deactivationTimerTurbo += Time.deltaTime;
        if (deactivationTimerTurbo > deactivationDelay)
        {
            turbo = false;
        }
    }
}

