using System;
using UnityEngine;

public class WaterCollector : MonoBehaviour
{
    public bool isConnected { get; private set; }
    public WaterSource connectedSource { get; private set; }

    public Action<WaterSource> onConnect;
    public Action<WaterSource> onDisconnect;

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