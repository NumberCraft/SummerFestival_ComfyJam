using UnityEngine;

public class WaterGun : MonoBehaviour, IPausable
{
    [Header("References")]
    [SerializeField] private WaterCollector collector;

    [Header("Stream Settings")]
    [SerializeField] private float streamRange = 15f;
    public float _streamRange
    {
        get { return streamRange; }
        private set { streamRange = value; }
    }

    [SerializeField] private LayerMask waterableLayer;
    [SerializeField] private float streamStrength = 0.2f;

    [Header("Player Rotation")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem muzzlePS;

    [Header("Blob Settings")]
    [SerializeField] private GameObject blobProjectilePrefab;
    [SerializeField] private Transform blobSpawnPoint;
    [SerializeField] private float blobFireRate = 0.4f;

    private float blobFireCooldown = 0f;

    public bool isShooting { get; private set; }
    private bool isStreaming = false;

    private void Start()
    {
        collector = GetComponentInParent<WaterCollector>();
    }

    private void Update()
    {
        // Must be connected to a water source
        if (!collector.isConnected)
        {
            StopStream();
            return;
        }

        HandleSingleStream();
        HandleBlob();
    }

    // ─────────────────────────────────────────────
    // LEFT CLICK = STREAM MODE
    // ─────────────────────────────────────────────


private void HandleSingleStream()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopStream();
        }
    }

    private void Shoot()
    {
        // STREAM ONLY
        isShooting = true;
        isStreaming = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, streamRange, waterableLayer))
        {
            if (hit.collider.TryGetComponent(out IWaterable waterable))
            {
                if (!waterable.IsFullyWatered())
                {
                    waterable.Water(streamStrength * Time.deltaTime);
                }
            }
            else if (hit.collider.TryGetComponent(out IMoveable moveable))
            {
                moveable.Move(ray.direction);
            }
        }

        if (!muzzlePS.isPlaying)
            muzzlePS.Play();
    }

    private void HandleBlob()
    {
        if (blobFireCooldown > 0f)
            blobFireCooldown -= Time.deltaTime;

        // RIGHT CLICK
        if (Input.GetMouseButton(1))
        {
            // Stop stream if active
            if (isShooting)
                StopStream();

            ShootBlob();
        }
    }

    private void ShootBlob()
    {
        if (blobFireCooldown > 0f)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, streamRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint =
                ray.origin + ray.direction * streamRange;
        }

        if (blobProjectilePrefab != null &&
            blobSpawnPoint != null)
        {
            Vector3 direction =
                (targetPoint - blobSpawnPoint.position)
                .normalized;

            GameObject blob = Instantiate(
                blobProjectilePrefab,
                blobSpawnPoint.position,
                Quaternion.LookRotation(direction)
            );

            if (blob.TryGetComponent(
                out WaterBlobProjectile projectile))
            {
                projectile.Launch(direction);
            }

            // IMPORTANT:
            // Blob mode should NOT activate stream visuals
            isShooting = false;
            isStreaming = false;

            blobFireCooldown = blobFireRate;

            if (!muzzlePS.isPlaying)
                muzzlePS.Play();
        }
    }

    private void StopStream()
    {
        isShooting = false;
        isStreaming = false;

        muzzlePS.Stop();
    }



    // ─────────────────────────────────────────────
    // RIGHT CLICK = BLOB MODE
    // ─────────────────────────────────────────────

    

    

    // ─────────────────────────────────────────────
    // Shared
    // ─────────────────────────────────────────────

    

    public void OnFlowerFullyWatered()
    {
        StopStream();
    }

    // ─────────────────────────────────────────────
    // Pause System
    // ─────────────────────────────────────────────

    public void Pause()
    {
        isShooting = false;
        enabled = false;
    }

    public void Continue()
    {
        enabled = true;
    }

    // ─────────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(
            0.2f,
            0.6f,
            1f,
            0.1f
        );

        Gizmos.DrawSphere(
            transform.position,
            streamRange
        );

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            streamRange
        );
    }
}

