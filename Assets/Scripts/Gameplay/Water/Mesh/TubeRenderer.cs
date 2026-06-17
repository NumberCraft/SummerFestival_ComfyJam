using System.Collections.Generic;
using UnityEngine;

public class TubeRenderer : MonoBehaviour
{
    [Header("Tube")]
    public int sides = 10;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    Vector3 referenceUp = Vector3.up;

    public void BuildMesh(List<Vector3> points, Mesh mesh, Transform meshTransform, float radius, float stress)
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        if (mesh == null)
        {
            Debug.Log(mesh + " is null.");

            return;
        }

        //float stress = GetStress();
        float dynamicRadius = radius * (1f - stress * 0.7f);

        int ringSize = sides;

        for (int i = 0; i < points.Count; i++)
        {
            float alongTube = i / (float)(points.Count - 1);

            float tipStart = 0.75f;
            float tipScale = 1f;

            if (alongTube > tipStart)
            {
                float t = Mathf.InverseLerp(tipStart, 1f, alongTube);

                // Smooth dome falloff (prevents sharp cone collapse)
                float dome = 1f - (t * t);

                // Never let it fully collapse
                float minTip = 0.4f;

                tipScale = Mathf.Lerp(minTip, 1f, dome);

                // Small “fluid bulge” before the tip
                tipScale *= (1f + Mathf.Sin(t * Mathf.PI) * 0.15f);
            }

            Vector3 delta;

            if (i < points.Count - 1)
                delta = points[i + 1] - points[i];
            else
                delta = points[i] - points[i - 1];

            if (delta.sqrMagnitude < 0.000001f)
                delta = Vector3.forward;

            Vector3 forward = delta.normalized;

            Vector3 side = Vector3.Cross(referenceUp, forward).normalized;

            if (side.sqrMagnitude < 0.001f)
                side = Vector3.Cross(Vector3.right, forward).normalized;

            Vector3 up = Vector3.Cross(forward, side).normalized;

            for (int j = 0; j < sides; j++)
            {
                float angle = (j / (float)sides) * Mathf.PI * 2f;

                Vector3 offset = (side * Mathf.Cos(angle) +
                    up * Mathf.Sin(angle))
                    * dynamicRadius * tipScale;

                //vertices.Add(points[i] + offset);
                vertices.Add(meshTransform.InverseTransformPoint(points[i] + offset));

                float u = j / (float)(sides - 1);
                float v = i / (float)(points.Count - 1);

                uvs.Add(new Vector2(u, v));
            }
        }

        for (int i = 0; i < points.Count - 1; i++)
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
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
