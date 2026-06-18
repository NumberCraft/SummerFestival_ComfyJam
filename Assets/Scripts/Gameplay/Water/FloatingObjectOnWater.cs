using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObjectOnWater : MonoBehaviour
{
    public Transform[] floaters;
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airWaterDrag = 0f;
    public float airWaterAngularDrag = 0.05f;

    public float floatingPower = 15f;

    public float baseWaterHeight = 0f;
    public float waterHeightVariation = 2f;
    public float waveSpeed = 1.0f;
    public float waterHeight;

    [Header("Water Check")]
    [SerializeField] private float waterCheckDistance = 1.2f;
    [SerializeField] private LayerMask waterMask;

    private bool isInWater;

    Rigidbody rb;
    int floatersUnderwater;
    bool underwater;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, waterCheckDistance, waterMask))
        {
            if (hit.collider.tag != "Water")
            {
                return;
            }
        }
        else
        {
            return;
        }*/

        if (!isInWater)
            return;

        waterHeight = baseWaterHeight + Mathf.Sin(Time.time * waveSpeed) * (waterHeightVariation / 2f);

        floatersUnderwater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
            float diff = floaters[i].position.y - waterHeight;

            if (diff < 0)
            {
                rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(diff), floaters[i].position, ForceMode.Force);
                floatersUnderwater++;
                if (!underwater)
                {
                    underwater = true;
                    SwitchState(true);
                }
            }
        }


        if (underwater && floatersUnderwater == 0)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            isInWater = false;
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            rb.linearDamping = underWaterDrag;
            rb.angularDamping = underWaterAngularDrag;
        }
        else
        {
            rb.linearDamping = airWaterDrag;
            rb.angularDamping = airWaterAngularDrag;
        }
    }
}
