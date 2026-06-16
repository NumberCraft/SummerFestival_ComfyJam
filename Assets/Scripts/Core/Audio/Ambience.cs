using UnityEngine;

public class Ambience : MonoBehaviour
{
    public Collider area;

    private void Update()
    {
        if (Camera.main == null)
            return;

        Vector3 closestPoint = area.ClosestPoint(Camera.main.transform.position);
        transform.position = closestPoint;
    }
}
