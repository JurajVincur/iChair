using System;
using UnityEngine;
using UnityEngine.Events;
using MixedReality.Toolkit.Input;

public class FixationDetector : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private GazeInteractor gazeInteractor;

    [Header("Keyboard Plane")]
    [SerializeField] private Transform keyboardTransform;

    [Header("Fixation Settings")]
    [SerializeField] private float minFixationDuration = 0.1f;
    [SerializeField] private float maxAngularDeviationDeg = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool logAngles = true;

    [Serializable]
    public class FixationStartedEvent : UnityEvent<Vector3, Vector3, Vector3> { }

    [Serializable]
    public class FixationEndedEvent : UnityEvent<Vector3, Vector3, Vector3, float> { }

    public FixationStartedEvent OnFixationStarted;
    public FixationEndedEvent OnFixationEnded;

    private bool isTracking = false;
    private bool fixationActive = false;

    private Vector3 anchorOrigin;
    private Vector3 anchorDirection;
    private Vector3 anchorFixationPoint;

    private Vector3 lastOrigin;
    private Vector3 lastDirection;
    private Vector3 lastFixationPoint;

    private float startTime;
    private float lastTime;

    void Update()
    {
        if (gazeInteractor == null || keyboardTransform == null) return;

        Vector3 rayOrigin = gazeInteractor.transform.position;
        Vector3 rayDirection = gazeInteractor.transform.forward.normalized;
        float now = Time.time;

        Vector3 fixationPoint = GetFixationPointOnKeyboardPlane(rayOrigin, rayDirection);

        if (!isTracking)
        {
            StartTracking(rayOrigin, rayDirection, fixationPoint, now);
            return;
        }

        float angle = Vector3.Angle(anchorDirection, rayDirection);

        if (logAngles)
        {
            Debug.Log($"Angle from anchor: {angle:F3}");
        }

        if (angle <= maxAngularDeviationDeg)
        {
            lastOrigin = rayOrigin;
            lastDirection = rayDirection;
            lastFixationPoint = fixationPoint;
            lastTime = now;

            float duration = now - startTime;

            if (!fixationActive && duration >= minFixationDuration)
            {
                fixationActive = true;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                //Debug.Log(
                //    $"Fixation STARTED. World position: {anchorFixationPoint}, " +
                //    $"Local keyboard position: {keyboardTransform.InverseTransformPoint(anchorFixationPoint)}, " +
                //    $"Duration: {duration:F3}s at {timestamp}"
                //);

                OnFixationStarted?.Invoke(anchorOrigin, anchorDirection, anchorFixationPoint);
            }
        }
        else
        {
            EndFixationIfNeeded();
            StartTracking(rayOrigin, rayDirection, fixationPoint, now);
        }
    }

    private Vector3 GetFixationPointOnKeyboardPlane(Vector3 rayOrigin, Vector3 rayDirection)
    {
        Plane keyboardPlane = new Plane(keyboardTransform.forward, keyboardTransform.position);
        Ray ray = new Ray(rayOrigin, rayDirection);

        if (keyboardPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        // Fallback
        return rayOrigin + rayDirection * 1.1f;
    }

    private void StartTracking(Vector3 origin, Vector3 direction, Vector3 fixationPoint, float time)
    {
        isTracking = true;
        fixationActive = false;

        anchorOrigin = origin;
        anchorDirection = direction;
        anchorFixationPoint = fixationPoint;

        lastOrigin = origin;
        lastDirection = direction;
        lastFixationPoint = fixationPoint;

        startTime = time;
        lastTime = time;
    }

    private void EndFixationIfNeeded()
    {
        if (fixationActive)
        {
            float duration = lastTime - startTime;

            //Debug.Log(
            //    $"Fixation ENDED. World position: {lastFixationPoint}, " +
            //    $"Local keyboard position: {keyboardTransform.InverseTransformPoint(lastFixationPoint)}, " +
            //    $"Duration: {duration:F3}s"
            //);

            OnFixationEnded?.Invoke(lastOrigin, lastDirection, lastFixationPoint, duration);
        }

        fixationActive = false;
    }
}
