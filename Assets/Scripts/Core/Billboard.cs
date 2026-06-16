using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        //transform.forward = Camera.main.transform.forward;
        Transform camPos = Camera.main.transform;
        Vector3 oppositeDirection = transform.position - (camPos.position - transform.position);
        transform.LookAt(oppositeDirection);
    }
}
