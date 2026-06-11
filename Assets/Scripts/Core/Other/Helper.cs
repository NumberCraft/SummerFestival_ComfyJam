using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Helper
{
    public static Transform GetClosestTarget(GameObject[] targets, Vector3 pos)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = pos;
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 directionToTarget = targets[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = targets[i].transform;
            }
        }

        return bestTarget;
    }

    public static Transform GetClosestTarget(List<Transform> targets, Vector3 pos)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = pos;
        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 directionToTarget = targets[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = targets[i].transform;
            }
        }

        return bestTarget;
    }

    public static Transform GetClosestTarget(Transform[] targets, Vector3 pos)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = pos;
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 directionToTarget = targets[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = targets[i].transform;
            }
        }

        return bestTarget;
    }

    public static Transform GetClosestTarget(Collider2D[] targets, Vector3 pos)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = pos;
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 directionToTarget = targets[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = targets[i].transform;
            }
        }

        return bestTarget;
    }

    public static GameObject GetClosestItem(Collider[] hitColliders, Transform transform)
    {
        GameObject closestObject = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            // Optional: Exclude self if the script is on the object creating the sphere
            /*if (hitCollider.gameObject == gameObject)
            {
                continue;
            }*/

            Vector3 directionToTarget = hitCollider.transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestObject = hitCollider.gameObject;
            }
        }

        return closestObject;
    }

    public static int LayermaskToLayer(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }

    public static GameObject FindInChildren(GameObject gameObject, string name)
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
                return t.gameObject;
        }

        return null;
    }

    public static GameObject FindInChildren(Transform transform, string name)
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
                return t.gameObject;
        }

        return null;
    }

    public static GameObject FindInChildrenThatContains(Transform transform, string name)
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.Contains(name))
                return t.gameObject;
        }

        return null;
    }

    public static int GetSceneIndexByName(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return i;
        }

        Debug.LogWarning($"Scene with name '{sceneName}' not found in Build Settings.");
        return -1; // Not found
    }

    /// <summary>
    /// Attempts to find a component of type T on this GameObject
    /// or any of its parents. Returns true if found.
    /// </summary>
    public static bool TryGetComponentInParent<T>(GameObject obj, out T result) where T : Component
    {
        Transform current = obj.transform;

        while (current != null)
        {
            if (current.TryGetComponent<T>(out result))
                return true;

            current = current.parent;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Overload for components: behaves like Unity's methods.
    /// </summary>
    public static bool TryGetComponentInParent<T>(Component comp, out T result) where T : Component
    {
        return TryGetComponentInParent<T>(comp.gameObject, out result);
    }
}
