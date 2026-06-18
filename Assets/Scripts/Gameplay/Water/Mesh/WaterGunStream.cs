using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterGunStream : MonoBehaviour
{
    public Transform shootPoint;
    public Transform target;
    public LayerMask hitMask;

    [SerializeField] private TubeRenderer tubeRenderer;
    [SerializeField] private WaterGun waterGun;

    public int resolution = 20;

    [Header("Tube")]
    [Range(0f, 1f)]
    public float streamProgress = 0f;

    [SerializeField] float streamLength = 0.25f;
    [SerializeField] float fillSpeed = 3f;
    [SerializeField] float drainSpeed = 2f;

    bool wasShooting;
    float releaseProgress;
    bool isReleasing;

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

        if (tubeRenderer == null)
            tubeRenderer = GetComponent<TubeRenderer>();
    }
    
    private void Update()
    {
        if (waterGun.isShooting)
        {
            isReleasing = false;
            wasShooting = true;

            // 1. Position the target via Raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, waterGun._streamRange, hitMask))
                target.position = hit.point;
            else
                target.position = ray.origin + ray.direction * waterGun._streamRange;

            // 2. Advance the front of the beam forward
            streamProgress = Mathf.MoveTowards(streamProgress, 1f, fillSpeed * Time.deltaTime);
            releaseProgress = 0f; // Reset release while shooting

            // 3. Generate points: Start is pinned at 0, End grows to 1
            List<Vector3> points = GeneratePoints(shootPoint.position, target.position, 0f, streamProgress);

            tubeRenderer.BuildMesh(points, waterMesh, waterMeshFilter.transform, waterRadius, stress);
            waterMeshRenderer.enabled = true;

            // Splash only plays when the tip actually reaches near the target
            UpdateSplashVFX(points, streamProgress >= 0.95f);
        }
        else
        {
            if (wasShooting)
            {
                if (!isReleasing)
                {
                    isReleasing = true;
                }

                // Advance the tail of the beam forward
                releaseProgress = Mathf.MoveTowards(releaseProgress, 1f, drainSpeed * Time.deltaTime);

                // Generate points: Start drains toward 1, End stays pinned at streamProgress (where it left off)
                List<Vector3> points = GeneratePoints(shootPoint.position, target.position, releaseProgress, streamProgress);

                tubeRenderer.BuildMesh(points, waterMesh, waterMeshFilter.transform, waterRadius, stress);
                waterMeshRenderer.enabled = true;

                // Keep splash active until the tail completely finishes draining
                UpdateSplashVFX(points, releaseProgress < 0.95f);

                // Clear stream entirely once the tail reaches the front
                if (releaseProgress >= streamProgress)
                {
                    ResetStreamState();
                }
            }
            else
            {
                if (waterMeshRenderer.enabled || splashVFX.activeSelf)
                {
                    ResetStreamState();
                }
            }
        }
    }

    private void UpdateSplashVFX(List<Vector3> points, bool shouldBeActive)
    {
        if (shouldBeActive && points != null && points.Count > 1)
        {
            Vector3 tip = points[^1];
            Vector3 prev = points[^2];

            splashVFX.transform.position = tip;
            splashVFX.transform.forward = (tip - prev).normalized;

            if (!splashVFX.activeSelf)
                splashVFX.SetActive(true);
        }
        else
        {
            splashVFX.SetActive(false);
        }
    }

    private void ResetStreamState()
    {
        streamProgress = 0f;
        releaseProgress = 0f;
        isReleasing = false;
        wasShooting = false; // Reset this so the else block stops processing

        waterMeshRenderer.enabled = false;
        splashVFX.SetActive(false);
    }

    List<Vector3> GeneratePoints(Vector3 a, Vector3 b, float startLerp, float endLerp)
    {
        List<Vector3> pts = new List<Vector3>();

        for (int i = 0; i <= resolution; i++)
        {
            float normalized = i / (float)resolution;

            // Map resolution points dynamically between our custom start and end values
            float t = Mathf.Lerp(startLerp, endLerp, normalized);
            t = Mathf.Clamp01(t);

            Vector3 point = CalculateBezierPoint(t, a, a, b, b);

            // Wobble scales down at the nozzle (t=0) and up at the tip
            float wobbleFactor = Mathf.SmoothStep(0f, 1f, t);
            float wobble = stress * stress * 0.5f * wobbleFactor;

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
