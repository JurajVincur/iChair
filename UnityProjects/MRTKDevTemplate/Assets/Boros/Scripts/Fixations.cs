using PupilLabs;
using System;
using UnityEngine;

public class Fixations : MonoBehaviour
{
    [SerializeField]
    private DeviceManager deviceManager;
    [SerializeField]
    private DataStorage storage;

    private RTSPWorker worker = null;
    private EyeEventData eyeEventData;
    private float[] gazeEvent = new float[10];

    void Update()
    {
        if (worker == null)
        {
            string deviceIp = deviceManager.SelectedDeviceIp;
            if (deviceIp != null)
            {
                if (storage == null || storage.Ready == false)
                {
                    return;
                }
                string url = $"rtsp://{deviceIp}:{storage.Config.rtspSettings.port}";
                worker = RTSPServiceWrapper.StartWorker<RTSPWorker>(url, (byte)(1 << (int)StreamId.EyeEvents));
                worker.DataReceived += OnDataReceived;
                worker.LogMessageReceived += (message) =>
                {
                    Debug.Log($"Worker Log: {message}");
                };
            }
        }
    }

    private void OnDataReceived(long timestampMs, bool rtcpSynchronized, byte streamId, byte payloadFormat, uint dataSize, IntPtr data)
    {
        if (streamId == (int)StreamId.EyeEvents)
        {
            EyeEventType eventType;
            long startTimeNs;
            long endTimeNs;
            EyeEventDataType dataType = RTSPServiceWrapper.BytesToEyeEventData(data, dataSize, 0, out eventType, out startTimeNs, out endTimeNs, gazeEvent);
            eyeEventData.SetData(dataType, eventType, startTimeNs, endTimeNs, gazeEvent, timestampMs, rtcpSynchronized);
            if (dataType == EyeEventDataType.FixationOnsetData)
            {
                if (eventType == EyeEventType.FixationOnset)
                {
                    Debug.Log("Fixation started");
                }
                else if (eventType == EyeEventType.SaccadeOnset)
                {
                    Debug.Log("Fixation ended");
                }
            }
            else if (dataType == EyeEventDataType.FixationData)
            {
                Debug.Log($"Fixation data - Start: {eyeEventData.startGazePoint}, End: {eyeEventData.endGazePoint}, Mean: {eyeEventData.meanGazePoint}, Amplitude (pixels): {eyeEventData.amplitudePixels}, Amplitude (degrees): {eyeEventData.amplitudeAngleDeg}, Mean Velocity: {eyeEventData.meanVelocity}, Max Velocity: {eyeEventData.maxVelocity}");
            }
        }
    }

    private void OnDestroy()
    {
        worker?.Dispose();
    }
}
