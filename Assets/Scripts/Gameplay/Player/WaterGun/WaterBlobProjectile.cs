using UnityEngine;

public class WaterBlobProjectile : MonoBehaviour
{
    [Header("Blob Settings")]
    [SerializeField] private float waterAmount = 0.25f;

    [Header("Launch Settings")]
    public float launchForce = 25f;
    public float upwardForce = 8f;

    [Header("Lifetime")]
    [SerializeField] private float maxLifetime = 8f;
    [SerializeField] private int maxCollisions = 3;
    [SerializeField] private bool explodeOnTouch = false;

    int collisions;

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
        //Destroy(gameObject, destroyAfter);

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

    private void Update()
    {
        if (collisions > maxCollisions)
            Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0f)
            Explode();
    }

    private void Explode()
    {
        if (splashEffect != null)
        {
            GameObject splash = Instantiate(splashEffect, transform.position, Quaternion.identity);

            Destroy(splash, 2f);
        }

        Destroy(gameObject);
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
        collisions++;

        //if (hasHit) return;
        //hasHit = true;

        // Give water
        if (collision.collider.TryGetComponent(out IWaterable waterable))
        {
            if (!waterable.IsFullyWatered())
            {
                waterable.Water(waterAmount);
            }

            Explode();
        }
        /*else if (collision.collider.TryGetComponent(out IMoveable moveable))
        {
            
        }*/
        else if (collision.collider.TryGetComponent(out IBlobHitable blobHitable))
        {
            blobHitable.Hit();
        }
        else
        {
            if (explodeOnTouch)
            {
                Explode();
            }
        }
    }
}
