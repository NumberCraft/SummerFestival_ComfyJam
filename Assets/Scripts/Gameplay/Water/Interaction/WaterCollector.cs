using System;
using UnityEngine;

public class WaterCollector : MonoBehaviour
{
    public bool isConnected { get; private set; }
    public WaterSource connectedSource { get; private set; }

    public Action<WaterSource> onConnect;
    public Action<WaterSource> onDisconnect;
    public Action<float> onDistanceChange;

    private void Update()
    {
        if (!isConnected)
            return;

        float distance = Vector3.Distance(transform.position, connectedSource.transform.position);

        // Returns 1 when right on top (0 distance) and 0 at maxRange
        float zeroToOne = Mathf.InverseLerp(0f, connectedSource._maxRange, distance);

        onDistanceChange?.Invoke(zeroToOne);
        //onDistanceChange?.Invoke(connectedSource._maxRange / Vector3.Distance(transform.position, connectedSource.transform.position));
    }

    public void Connect(WaterSource source)
    {
        connectedSource = source;
        isConnected = true;
        Debug.Log("Connected");

        onConnect?.Invoke(connectedSource);
    }

    public void Disconnect()
    {
        onDisconnect?.Invoke(connectedSource);

        connectedSource = null;
        isConnected = false;
        Debug.Log("Disconnected");
    }
}