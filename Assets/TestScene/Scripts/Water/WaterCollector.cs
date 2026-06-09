using UnityEngine;

public class WaterCollector : MonoBehaviour
{
    public ParticleSystem waterStream;

    [Header("Manual Position Offset")]
    public Vector3 streamOffset;

    private Transform currentWaterSource;
    private bool isConnected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterSource"))
        {
            currentWaterSource = other.transform;
            isConnected = true;

            waterStream.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WaterSource"))
        {
            currentWaterSource = null;
            isConnected = false;

            waterStream.Stop();
        }
    }

    private void Update()
    {
        if (!isConnected || currentWaterSource == null)
            return;

        // Keep your manual position
        waterStream.transform.position =
            transform.position + streamOffset;

        // Rotate toward water source
        Vector3 direction =
            (currentWaterSource.position -
            waterStream.transform.position).normalized;

        waterStream.transform.rotation =
            Quaternion.LookRotation(direction);
    }
}