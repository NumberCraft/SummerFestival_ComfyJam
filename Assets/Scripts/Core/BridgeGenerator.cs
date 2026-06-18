using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class BridgeGenerator : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer;

    [Header("Bridge Parts")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject middlePrefab;
    [SerializeField] private GameObject endPrefab;

    [Header("Settings")]
    [SerializeField] private float middleSegmentLength = 5f;
    [SerializeField] private bool clearOldPieces = true;

    [ContextMenu("Generate Bridge")]
    public void GenerateBridge()
    {
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer missing.");
            return;
        }

        if (middlePrefab == null)
        {
            Debug.LogError("Middle prefab missing.");
            return;
        }

        if (clearOldPieces)
        {
            ClearChildren();
        }

        float splineLength = splineContainer.CalculateLength();

        // START
        if (startPrefab != null)
        {
            CreatePiece(startPrefab, 0f);
        }

        // MIDDLE SEGMENTS
        int middleCount = Mathf.FloorToInt(splineLength / middleSegmentLength);

        for (int i = 1; i < middleCount; i++)
        {
            float distance = i * middleSegmentLength;
            float t = distance / splineLength;

            CreatePiece(middlePrefab, t);
        }

        // END
        if (endPrefab != null)
        {
            CreatePiece(endPrefab, 1f);
        }
    }

    private void CreatePiece(GameObject prefab, float t)
    {
        Vector3 position = splineContainer.EvaluatePosition(t);

        Vector3 tangent = ((Vector3)splineContainer.EvaluateTangent(t)).normalized;

        Quaternion rotation =
            Quaternion.LookRotation(tangent, Vector3.up);

        GameObject obj;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(
                prefab,
                transform);
        }
        else
#endif
        {
            obj = Instantiate(prefab, transform);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
#endif
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
