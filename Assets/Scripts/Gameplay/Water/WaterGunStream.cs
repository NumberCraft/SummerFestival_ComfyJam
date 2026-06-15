using System.Collections.Generic;
using UnityEngine;

public class WaterGunStream : MonoBehaviour
{
    public Transform shootPoint;
    public Transform target;
    public LayerMask hitMask;

    [SerializeField] private TubeRenderer tubeRenderer;
    [SerializeField] private WaterGun waterGun;

    public int resolution = 20;

    [Header("Water Mesh")]
    [SerializeField] private MeshFilter waterMeshFilter;
    [SerializeField] private MeshRenderer waterMeshRenderer;
    public float waterRadius = 0.1f;
    [SerializeField] private float stress;
    Mesh waterMesh;

    [Header("Water Splash")]
    [SerializeField] private GameObject splashVFX;

    private void Awake()
    {
        waterMesh = new Mesh();
        waterMesh.name = "WaterGunStreamMesh";
        waterMesh.MarkDynamic();
        waterMeshFilter.sharedMesh = waterMesh;

        tubeRenderer = GetComponent<TubeRenderer>();
    }

    private void Update()
    {
        if (!waterGun.isShooting)
        {
            waterMeshRenderer.enabled = false;

            splashVFX.SetActive(false);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, waterGun._streamRange, hitMask))
            if (Physics.Raycast(ray, out RaycastHit hit, waterGun._streamRange, hitMask))
            {
                target.position = hit.point;

                List<Vector3> points = GeneratePoints();

                tubeRenderer.BuildMesh(points, waterMesh, waterMeshFilter.transform, waterRadius, stress);

                waterMeshRenderer.enabled = true;

                splashVFX.transform.position = hit.point;
                splashVFX.transform.forward = hit.normal;
                splashVFX.SetActive(true);
            }
            else
            {
                splashVFX.SetActive(false);
            }
        }
    }

    List<Vector3> GeneratePoints()
    {
        List<Vector3> pts = new List<Vector3>();

        Vector3 a = shootPoint.position;
        Vector3 b = target.position;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 point = CalculateBezierPoint(t, a, a, b, b);

            float wobble = stress * stress * 0.5f;

            point += new Vector3(
                Mathf.Sin(Time.time * 10f + t * 5f) * wobble,
                Mathf.Sin(Time.time * 6f + t * 3f) * wobble,
                Mathf.Cos(Time.time * 12f + t * 5f) * wobble
            );

            pts.Add(point);
        }

        return pts;
    }

    Vector3 CalculateBezierPoint(float t, Vector3 a, Vector3 c1, Vector3 c2, Vector3 b)
    {
        float u = 1 - t;

        return
            (u * u * u) * a +
            (3 * u * u * t) * c1 +
            (3 * u * t * t) * c2 +
            (t * t * t) * b;
    }
}
