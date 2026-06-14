using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaterStream : MonoBehaviour
{
    public Transform source;
    public Transform target;

    [SerializeField] private WaterCollector collector;
    [SerializeField] private TubeRenderer tubeRenderer;

    [Header("Distance / Collapse")]
    public float maxDistance = 8f;
    public float stressDistance = 6f;

    public int resolution = 20;
    public float curveHeight = 2f;

    [Header("Stress Levels")]
    [SerializeField] private float stressedLevel = 0.1f;
    [SerializeField] private float breakingLevel = 0.2f;

    [Header("Tube")]
    public float bendStrength = 2f;
    public float bodyAvoidStrength = 1.5f;

    [Header("Water Mesh")]
    [SerializeField] private MeshFilter waterMeshFilter;
    [SerializeField] private MeshRenderer waterMeshRenderer;
    public float waterRadius = 0.1f;
    Mesh waterMesh;

    [Header("Force Mesh")]
    [SerializeField] private MeshFilter forceMeshFilter;
    [SerializeField] private MeshRenderer forceMeshRenderer;
    public float forceRadius = 0.15f;
    Mesh forceMesh;

    [Header("Water Splash")]
    [SerializeField] private GameObject splashPrefab;

    public enum StreamState
    {
        Stable,
        Stressed,
        Breaking,
        Broken,
        NotConnected,
    }

    public StreamState state;
    public StreamState previousState;

    private bool isConnected;

    void Awake()
    {
        waterMesh = new Mesh();
        waterMesh.name = "WaterMesh";
        waterMesh.MarkDynamic();
        waterMeshFilter.sharedMesh = waterMesh;

        forceMesh = new Mesh();
        forceMesh.name = "ForceMesh";
        forceMesh.MarkDynamic();
        forceMeshFilter.sharedMesh = forceMesh;

        tubeRenderer = GetComponent<TubeRenderer>();
        //collector = GetComponentInParent<WaterCollector>();
    }

    void Update()
    {
        float distance = Vector3.Distance(source.position, target.position);

        previousState = state;

        if (distance < stressDistance)
            state = StreamState.Stable;
        else if (distance < maxDistance)
            state = StreamState.Stressed;
        else if (distance < maxDistance * 1.2f)
            state = StreamState.Breaking;
        else
            state = StreamState.Broken;

        if (collector.isConnected)
        {
            isConnected = true;

            List<Vector3> points = GeneratePoints();

            tubeRenderer.BuildMesh(points, waterMesh, waterMeshFilter.transform, waterRadius, GetStress());
            tubeRenderer.BuildMesh(points, forceMesh, forceMeshFilter.transform, forceRadius, GetStress());

            waterMeshRenderer.enabled = true;
            forceMeshRenderer.enabled = true;
        }
        else
        {
            if (isConnected)
            {
                state = StreamState.Broken;

                if (state == StreamState.Broken && previousState != StreamState.Broken)
                {
                    SpawnDroplets();

                    //forceMesh = null;
                    //forceMeshFilter.sharedMesh = null;

                    //waterMesh = null;
                    //waterMeshFilter.sharedMesh = null;
                }

                isConnected = false;
            }
            else
            {
                state = StreamState.NotConnected;
            }

            waterMeshRenderer.enabled = false;
            forceMeshRenderer.enabled = false;
        }
    }

    float GetStress()
    {
        switch (state)
        {
            case StreamState.Stable: return 0f;
            case StreamState.Stressed: return stressedLevel;
            case StreamState.Breaking: return breakingLevel;
            case StreamState.Broken: return 1f;
        }
        return 0f;
    }

    List<Vector3> GeneratePoints()
    {
        float stress = GetStress();

        List<Vector3> pts = new List<Vector3>();

        Vector3 a = source.position;
        Vector3 b = target.position;

        // correct flow direction (NOT transform forward)
        Vector3 endDir = (source.position - target.position).normalized;
        Vector3 playerForward = endDir;

        Vector3 controlA = a + Vector3.up * curveHeight;

        Vector3 sideOffset =
            Vector3.Cross(playerForward, Vector3.up).normalized
            * bodyAvoidStrength;

        Vector3 midControl =
            (a + b) * 0.5f
            + sideOffset
            + Vector3.up * (curveHeight * 0.5f);

        Vector3 controlB =
            b - endDir * bendStrength
            + Vector3.up * curveHeight * 0.3f;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 point = CalculateBezierPoint(t, a, controlA, midControl, b);

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

    void SpawnDroplets()
    {
        Debug.Log("STREAM BROKEN → spawn droplets here");

        List<Vector3> points = GeneratePoints();

        for (int i = 0; i < points.Count - 2; i++)
        {
            GameObject vfx = Instantiate(splashPrefab, points[i], Quaternion.identity);

            vfx.GetComponentInChildren<VisualEffect>().Play();

            Destroy(vfx.gameObject, 5f);
        }
    }

    /*public Transform source;
    public Transform target;

    public float maxDistance = 8f;
    public float stressDistance = 6f;

    public bool isCollapsing;

    bool connected = true;

    public int resolution = 20;
    public float curveHeight = 2f;

    [Header("Tube")]
    public int sides = 10;
    public float bendStrength = 2f;
    public float bodyAvoidStrength = 1.5f;

    [Header("WaterMesh")]
    [SerializeField] private MeshFilter waterMeshFilter;
    [SerializeField] private MeshRenderer waterMeshRenderer;
    public float waterRadius = 0.1f;

    Mesh waterMesh;

    [Header("ForceMesh")]
    [SerializeField] private MeshFilter forceMeshFilter;
    [SerializeField] private MeshRenderer forceMeshRenderer;
    public float forceRadius = 0.15f;

    Mesh forceMesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    Vector3 referenceUp = Vector3.up;

    List<Vector2> uvs = new List<Vector2>();


    public enum StreamState
    {
        Stable,
        Stressed,
        Breaking,
        Broken
    }

    public StreamState state;
    public StreamState previousState;


    void Awake()
    {
        if (waterMeshFilter == null || forceMeshFilter == null)
        {
            Debug.LogError("MeshFilter missing on WaterStream!");
            return;
        }

        waterMesh = new Mesh();
        waterMesh.name = "WaterStreamMesh";

        waterMesh.MarkDynamic();

        waterMeshFilter.sharedMesh = waterMesh;


        forceMesh = new Mesh();
        forceMesh.name = "ForceStreamMesh";
        forceMesh.MarkDynamic();

        forceMeshFilter.sharedMesh = forceMesh;
    }

    void Update()
    {
        float distance = Vector3.Distance(source.position, target.position);

        previousState = state;

        if (distance < stressDistance)
            state = StreamState.Stable;
        else if (distance < maxDistance)
            state = StreamState.Stressed;
        else if (distance < maxDistance * 1.2f)
            state = StreamState.Breaking;
        else
            state = StreamState.Broken;

        if (state == StreamState.Broken && previousState != StreamState.Broken)
        {
            SpawnDroplets();
        }

        List<Vector3> points = GeneratePoints();

        BuildMesh(points, waterMesh, waterRadius);
        BuildMesh(points, forceMesh, forceRadius);
    }

    float GetStress()
    {
        switch (state)
        {
            case StreamState.Stable: return 0f;
            case StreamState.Stressed: return 0.3f;
            case StreamState.Breaking: return 0.7f;
            case StreamState.Broken: return 1f;
        }
        return 0f;
    }

    List<Vector3> GeneratePoints()
    {
        float stress = (state == StreamState.Broken) ? 1f :
               (state == StreamState.Breaking) ? 0.7f :
               (state == StreamState.Stressed) ? 0.3f : 0f;

        List<Vector3> pts = new List<Vector3>();

        Vector3 a = source.position;
        Vector3 b = target.position;

        Vector3 endDir = (source.position - target.position).normalized;
        Vector3 playerForward = endDir;

        // control near source
        Vector3 controlA = a + Vector3.up * curveHeight;

        // ---- NEW: body avoidance offset ----
        // pushes curve sideways relative to player facing direction
        Vector3 sideOffset = Vector3.Cross(playerForward, Vector3.up).normalized * bodyAvoidStrength;

        // mid control point gets pushed away from player body
        Vector3 midControl = (a + b) * 0.5f + sideOffset + Vector3.up * (curveHeight * 0.5f);

        // control near player (still ensures correct entry direction)
        Vector3 controlB = b - endDir * bendStrength + Vector3.up * curveHeight * 0.3f;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 point = CalculateBezierPoint(t, a, controlA, midControl, b);

            // collapse wobble (applied BEFORE storing)
            float wobble = stress * 0.5f;
            //float wobble = collapseProgress * collapseProgress * 0.5f;

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

    public void BuildMesh(List<Vector3> points, Mesh mesh, float radius)
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        int ringCount = points.Count;
        int ringSize = sides;

        float stress = GetStress();

        float dynamicRadius = radius * (1f - stress * 0.7f);

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 forward;

            if (i < points.Count - 1)
                forward = (points[i + 1] - points[i]).normalized;
            else
                forward = (points[i] - points[i - 1]).normalized;

            Vector3 side = Vector3.Cross(referenceUp, forward).normalized;

            // fallback if forward is too close to up vector
            if (side.sqrMagnitude < 0.001f)
            {
                side = Vector3.Cross(Vector3.right, forward).normalized;
            }

            Vector3 up = Vector3.Cross(forward, side).normalized;

            for (int j = 0; j < sides; j++)
            {
                float angle = (j / (float)sides) * Mathf.PI * 2f;

                Vector3 offset =
                    (side * Mathf.Cos(angle) + up * Mathf.Sin(angle))
                    * dynamicRadius;

                vertices.Add(points[i] + offset);

                Vector3 normal = (side * Mathf.Cos(angle) + up * Mathf.Sin(angle)).normalized;
                normals.Add(-normal);

                float u = j / (float)(sides - 1);
                float v = i / (float)(points.Count - 1);

                uvs.Add(new Vector2(u, v));
            }
        }

        for (int i = 0; i < ringCount - 1; i++)
        {
            int ringStart = i * ringSize;
            int nextRingStart = (i + 1) * ringSize;

            for (int j = 0; j < ringSize - 1; j++)
            {
                int current = ringStart + j;
                int next = nextRingStart + j;

                int currentNext = current + 1;
                int nextNext = next + 1;

                triangles.Add(current);
                triangles.Add(next);
                triangles.Add(currentNext);

                triangles.Add(currentNext);
                triangles.Add(next);
                triangles.Add(nextNext);
            }

            int last = ringStart + ringSize - 1;
            int lastNext = nextRingStart + ringSize - 1;

            int first = ringStart;
            int firstNext = nextRingStart;

            triangles.Add(last);
            triangles.Add(lastNext);
            triangles.Add(first);

            triangles.Add(first);
            triangles.Add(lastNext);
            triangles.Add(firstNext);
        }

        if (vertices.Count != uvs.Count)
        {
            Debug.LogError($"Mismatch V:{vertices.Count} UV:{uvs.Count}");
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
    }

    void OnDrawGizmos()
    {
        if (source == null || target == null)
            return;

        Gizmos.color = Color.cyan;

        Vector3 prev = source.position;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 point = Vector3.Lerp(source.position, target.position, t);
            point += Vector3.up * Mathf.Sin(t * Mathf.PI) * curveHeight;

            Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }*/
}
