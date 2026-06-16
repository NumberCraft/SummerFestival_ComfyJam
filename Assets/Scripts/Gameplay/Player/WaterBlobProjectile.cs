using UnityEngine;

public class WaterBlobProjectile : MonoBehaviour
{
    [Header("Blob Settings")]
    [SerializeField] private float destroyAfter = 5f;
    [SerializeField] private float waterAmount = 0.25f;

    [Header("Launch Settings")]
    public float launchForce = 25f;
    public float upwardForce = 8f;

    [Header("Effects")]
    [SerializeField] private GameObject splashEffect;

    private Rigidbody rb;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, destroyAfter);

        // Ignore player collision
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Collider playerCollider =
                player.GetComponent<Collider>();

            Collider myCollider =
                GetComponent<Collider>();

            if (playerCollider != null &&
                myCollider != null)
            {
                Physics.IgnoreCollision(
                    myCollider,
                    playerCollider
                );
            }
        }
    }

    public void Launch(Vector3 direction)
    {
        if (rb == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Add upward arc
        Vector3 launchDirection =
            (direction + Vector3.up * upwardForce)
            .normalized;

        rb.AddForce(
            launchDirection * launchForce,
            ForceMode.Impulse
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        FlowerController flower =
            collision.collider
            .GetComponent<FlowerController>();

        // Give water
        if (flower != null &&
            !flower.isFullyWatered)
        {
            flower.AddWaterAmount(
                waterAmount
            );
        }

        // Splash effect
        if (splashEffect != null)
        {
            GameObject splash =
                Instantiate(
                    splashEffect,
                    transform.position,
                    Quaternion.identity
                );

            Destroy(splash, 2f);
        }

        Destroy(gameObject);
    }
}
