using UnityEngine;

public class MoveableObject : MonoBehaviour, IMoveable
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float forcePower = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 dir)
    {
        rb.AddForce(dir * forcePower, ForceMode.Impulse);
    }
}
