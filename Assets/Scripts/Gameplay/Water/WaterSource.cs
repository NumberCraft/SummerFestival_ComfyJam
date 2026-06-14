using UnityEngine;

public class WaterSource : MonoBehaviour
{
    [Header("Range Settings")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float maxRange = 10f;

    [Header("Keybinds")]
    [SerializeField] private KeyCode connectKey = KeyCode.F;

    private WaterCollector playerCollector;

    private void Start()
    {
        playerCollector = FindAnyObjectByType<WaterCollector>();
    }

    private void Update()
    {
        if (playerCollector == null) return;

        float distance = Vector3.Distance(transform.position, playerCollector.transform.position);

        if (!playerCollector.isConnected)
        {
            if (distance <= pickupRange && Input.GetKeyDown(connectKey))
            {
                playerCollector.Connect(this);
                Debug.Log("Connected to " + gameObject.name);
            }
        }
        else if (playerCollector.connectedSource == this)
        {
            // Break connection if player exceeds max range
            if (distance > maxRange)
            {
                playerCollector.Disconnect();
                Debug.Log("Disconnected - out of range");
            }

            // Manual disconnect
            if (Input.GetKeyDown(connectKey))
            {
                playerCollector.Disconnect();
                Debug.Log("Disconnected - manual");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Pickup range - green
        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, pickupRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        // Max range - red
        Gizmos.color = new Color(1f, 0f, 0f, 0.05f);
        Gizmos.DrawSphere(transform.position, maxRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}