using UnityEngine;

public enum GunMode
{
    None,
    SingleStream,
    Blob
}

public class WaterGun : MonoBehaviour
{
    [Header("Mode")]
    public GunMode currentMode = GunMode.None;

    [Header("Stream Settings")]
    [SerializeField] private ParticleSystem streamParticles;
    [SerializeField] private float streamRange = 15f;
    [SerializeField] private LayerMask flowerLayer;

    [Header("Blob Mode")]
    [SerializeField] private GameObject blobProjectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float blobCooldown = 0.4f;

    [Header("Player Rotation")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float rotationSpeed = 10f;

    private FlowerController currentTargetFlower;
    private bool isStreaming = false;

    private float nextBlobTime;

    private void Update()
    {
        HandleModeSwitch();

        switch (currentMode)
        {
            case GunMode.SingleStream:
                HandleSingleStream();
                break;

            case GunMode.Blob:
                HandleBlobMode();
                break;

            default:
                StopStream();
                break;
        }
    }

    private void HandleModeSwitch()
    {
        // Stream mode toggle
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode =
                currentMode == GunMode.SingleStream
                ? GunMode.None
                : GunMode.SingleStream;

            StopStream();
            HandleCursor();
        }

        // Blob mode toggle
        if (Input.GetMouseButtonDown(1))
        {
            currentMode =
                currentMode == GunMode.Blob
                ? GunMode.None
                : GunMode.Blob;

            StopStream();
            HandleCursor();
        }
    }

    private void HandleCursor()
    {
        bool aiming =
            currentMode == GunMode.SingleStream ||
            currentMode == GunMode.Blob;

        Cursor.visible = aiming;

        Cursor.lockState =
            aiming
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Debug.Log("Gun Mode: " + currentMode);
    }

    private void HandleSingleStream()
    {
        if (Input.GetMouseButtonDown(0))
            TrySelectFlower();

        if (Input.GetMouseButtonUp(0))
        {
            StopStream();
            return;
        }

        if (Input.GetMouseButton(0) &&
            isStreaming &&
            currentTargetFlower != null &&
            !currentTargetFlower.isFullyWatered)
        {
            RotatePlayerTowardFlower();

            if (!streamParticles.isPlaying)
                streamParticles.Play();

            currentTargetFlower.AddWater(Time.deltaTime);
        }
    }

    private void HandleBlobMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < nextBlobTime)
                return;

            nextBlobTime = Time.time + blobCooldown;

            ShootBlob();
        }
    }

    private void ShootBlob()
    {
        if (blobProjectilePrefab == null)
        {
            Debug.LogError("Blob Projectile Missing!");
            return;
        }

        if (shootPoint == null)
        {
            Debug.LogError("Shoot Point Missing!");
            return;
        }

        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint =
                ray.origin +
                ray.direction * 50f;
        }

        Vector3 direction =
            (targetPoint - shootPoint.position)
            .normalized;

        GameObject blob =
            Instantiate(
                blobProjectilePrefab,
                shootPoint.position +
                shootPoint.forward * 0.5f,
                Quaternion.identity
            );

        WaterBlobProjectile projectile =
            blob.GetComponent<WaterBlobProjectile>();

        if (projectile != null)
        {
            projectile.Launch(direction);
        }
    }

    private void TrySelectFlower()
    {
        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            Mathf.Infinity,
            flowerLayer))
        {
            FlowerController flower =
                hit.collider.GetComponent<FlowerController>();

            if (flower != null &&
                !flower.isFullyWatered)
            {
                float dist =
                    Vector3.Distance(
                        transform.position,
                        flower.transform.position);

                if (dist <= streamRange)
                {
                    currentTargetFlower = flower;
                    isStreaming = true;
                }
            }
        }
        else
        {
            StopStream();
        }
    }

    private void RotatePlayerTowardFlower()
    {
        if (playerBody == null ||
            currentTargetFlower == null)
            return;

        Vector3 direction =
            (
                currentTargetFlower.transform.position
                - playerBody.position
            ).normalized;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            playerBody.rotation =
                Quaternion.Slerp(
                    playerBody.rotation,
                    targetRotation,
                    Time.deltaTime *
                    rotationSpeed
                );
        }
    }

    private void StopStream()
    {
        currentTargetFlower = null;
        isStreaming = false;

        if (streamParticles != null &&
            streamParticles.isPlaying)
        {
            streamParticles.Stop();
        }
    }

    public void OnFlowerFullyWatered()
    {
        StopStream();
    }
}