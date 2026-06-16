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
            if (distance <= pickupRange)
            {
                if (Input.GetKeyDown(connectKey))
                {
                    playerCollector.Connect(this);
                    Debug.Log("Connected to " + gameObject.name);
                }

                InteractUIManager.Instance.Show(InteractType.Water);
            }
            else if (distance > pickupRange)
            {
                if (!IsNearToOther())
                    InteractUIManager.Instance.Hide(InteractType.Water);
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

            InteractUIManager.Instance.Hide(InteractType.Water);
        }
    }

    private bool IsNearToOther()
    {
        WaterSource[] waterSources = FindObjectsByType<WaterSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var ws in waterSources)
        {
            float distance = Vector3.Distance(ws.transform.position, playerCollector.transform.position);

            if (distance <= ws.pickupRange)
            {
                return true;
            }
        }

        return false;
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