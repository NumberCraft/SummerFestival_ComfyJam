using UnityEngine;

public class WaterGunController : MonoBehaviour
{
    /*[Header("Mode")]
    public GunMode currentMode = GunMode.None;

    [Header("Stream Settings")]
    [SerializeField] private ParticleSystem streamParticles;
    [SerializeField] private float streamRange = 15f;
    [SerializeField] private LayerMask flowerLayer;

    [Header("Player Rotation")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float rotationSpeed = 10f;

    private FlowerController currentTargetFlower;
    private bool isStreaming = false;

    public enum GunMode
    {
        None,
        SingleStream
    }

    private void Update()
    {
        HandleModeSwitch();

        if (currentMode == GunMode.SingleStream)
            HandleSingleStream();
        else
            StopStream();
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = currentMode == GunMode.SingleStream
                ? GunMode.None
                : GunMode.SingleStream;

            if (currentMode == GunMode.SingleStream)
            {
                // Unlock cursor so player can freely click on flowers
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                // Lock cursor back when leaving aim mode
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                StopStream();
            }

            Debug.Log("Gun Mode: " + currentMode);
        }
    }

    private void HandleSingleStream()
    {
        // Click to select a flower target
        if (Input.GetMouseButtonDown(0))
            TrySelectFlower();

        // Release to stop stream
        if (Input.GetMouseButtonUp(0))
        {
            StopStream();
            return;
        }

        // While holding mouse - stream toward locked target
        if (Input.GetMouseButton(0) && isStreaming && currentTargetFlower != null && !currentTargetFlower.isFullyWatered)
        {
            RotatePlayerTowardFlower();

            if (!streamParticles.isPlaying)
                streamParticles.Play();

            currentTargetFlower.AddWater(Time.deltaTime);
        }
    }

    private void TrySelectFlower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, flowerLayer))
        {
            FlowerController flower = hit.collider.GetComponent<FlowerController>();

            if (flower != null && !flower.isFullyWatered)
            {
                float dist = Vector3.Distance(transform.position, flower.transform.position);

                if (dist <= streamRange)
                {
                    currentTargetFlower = flower;
                    isStreaming = true;
                    Debug.Log("Targeting: " + flower.gameObject.name);
                }
                else
                {
                    Debug.Log("Flower out of range");
                }
            }
        }
        else
        {
            // Clicked on nothing - stop current stream
            StopStream();
        }
    }

    private void RotatePlayerTowardFlower()
    {
        if (playerBody == null || currentTargetFlower == null) return;

        Vector3 direction = (currentTargetFlower.transform.position - playerBody.position).normalized;
        direction.y = 0f; // Keep rotation flat, no tilting up/down

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void StopStream()
    {
        currentTargetFlower = null;
        isStreaming = false;

        if (streamParticles != null && streamParticles.isPlaying)
            streamParticles.Stop();
    }

    public void OnFlowerFullyWatered()
    {
        StopStream();
    }

    private void OnDrawGizmos()
    {
        // Changes color based on current mode
        if (currentMode == GunMode.SingleStream)
        {
            // Blue when aim mode is active
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.1f);
            Gizmos.DrawSphere(transform.position, streamRange);
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 1f);
            Gizmos.DrawWireSphere(transform.position, streamRange);
        }
        else
        {
            // Grey when mode is off
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.05f);
            Gizmos.DrawSphere(transform.position, streamRange);
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, streamRange);
        }
    }*/
}
