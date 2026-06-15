using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform orientation;

    [SerializeField] private MultiAimConstraint aimConstraint;

    [SerializeField] private float distance = 30f;
    [SerializeField] private float angle = 0.7f;
    [SerializeField] private float speed = 2f;

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        target.position = cam.position + cam.forward * distance;

        // 1. Get the direction from the observer to the target
        Vector3 directionToTarget = target.position - orientation.position;

        // 2. Normalize it (important for consistent results)
        directionToTarget.Normalize();

        // 3. Calculate the Dot Product between observer's forward and the direction
        float dot = Vector3.Dot(orientation.forward, directionToTarget);

        float targetWeight = (dot < angle) ? 0f : 1f;
        aimConstraint.weight = Mathf.MoveTowards(aimConstraint.weight, targetWeight, Time.deltaTime * speed);

        //UpdateRig(rigBuilder, animator);
    }
}
