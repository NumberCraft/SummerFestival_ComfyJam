using UnityEngine;

public class WaterCollector : MonoBehaviour
{
    public bool isConnected { get; private set; }
    public WaterSource connectedSource { get; private set; }

    public void Connect(WaterSource source)
    {
        connectedSource = source;
        isConnected = true;
        Debug.Log("Connected");
    }

    public void Disconnect()
    {
        connectedSource = null;
        isConnected = false;
        Debug.Log("Disconnected");

    }
}