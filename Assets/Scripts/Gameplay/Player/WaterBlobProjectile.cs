using UnityEngine;

public class WaterBlobProjectile : MonoBehaviour
{
    [Header("Blob Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float destroyAfter = 5f;
    [SerializeField] private float waterAmount = 0.25f;

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
        // Destroy automatically if nothing hit
        Destroy(gameObject, destroyAfter);

        // Ignore player collision
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            Collider myCollider = GetComponent<Collider>();

            if (playerCollider != null && myCollider != null)
            {
                Physics.IgnoreCollision(myCollider, playerCollider);
            }
        }
    }

    public void Launch(Vector3 direction)
    {
        if (rb == null) return;

        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Prevent double hit bug
        if (hasHit) return;
        hasHit = true;

        FlowerController flower =
            collision.collider.GetComponent<FlowerController>();

        // Give water
        if (flower != null && !flower.isFullyWatered)
        {
            flower.AddWaterAmount(waterAmount);
        }

        // Spawn splash effect
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

        //Destroy(gameObject);
    }
}