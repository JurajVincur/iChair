using PupilLabs;
using UnityEngine;

public class SliderStateController : BleController
{
    [SerializeField] GazeSlider horizontalSlider;
    [SerializeField] int maxExtentX = 64;
    [SerializeField] int maxExtentY = 64;
    [SerializeField] float downThreshold = -0.4f;
    [SerializeField] float upThreshold = 0.4f;

    public override void Stop()
    {
        horizontalSlider.ResetState();
    }

    private void Update()
    {
        ble.SetXY01(horizontalSlider.Value, 0f, maxExtentX, maxExtentY);
    }

    public override void OnGazeDataReady(GazeDataProvider gdp)
    {
        Vector3 direction = gdp.GazeRay.direction;
        float hY = direction.y / direction.z;
        Debug.Log($"hY: {hY}");

        base.OnGazeDataReady(gdp);
    }
}
