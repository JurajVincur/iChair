using PupilLabs;
using UnityEngine;

public abstract class BleController : MonoBehaviour
{
    [SerializeField] protected TestBle ble;
    [SerializeField] protected GazeDataProvider gazeDataProvider;

    protected float appertureThresholdMm = 5f;
    protected float appertureThresholdTime = 0.2f;
    protected float appertureTimer = 0f;

    public abstract void Stop();

    protected virtual void Start()
    {
        ServiceLocator sl = ServiceLocator.Instance;
        if (ble == null)
        {
            ble = sl.GetComponentInChildren<TestBle>();
        }
        if (gazeDataProvider == null)
        {
            gazeDataProvider = sl.GazeDataProvider;
        }
        gazeDataProvider.gazeDataReady.AddListener(OnGazeDataReady);
    }

    public virtual void OnGazeDataReady(GazeDataProvider gdp)
    {
        if (gazeDataProvider.EyelidAvailable)
        {
            Eyelid eyelid = gazeDataProvider.RawEyelid;
            float apperture = Mathf.Max(eyelid.eyelidApertureLeft, eyelid.eyelidApertureRight);
            if (apperture < appertureThresholdMm)
            {
                appertureTimer += Time.deltaTime;
            }
            else
            {
                appertureTimer = 0f;
            }
            if (appertureTimer > appertureThresholdTime)
            {
                Stop();
            }
        }
    }

    private void OnDestroy()
    {
        gazeDataProvider.gazeDataReady.RemoveListener(OnGazeDataReady);
    }
}
