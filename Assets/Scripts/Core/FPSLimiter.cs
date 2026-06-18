using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 60;

    void Awake()
    {
        // 1. Disable VSync (Required for custom frame rates to work)
        QualitySettings.vSyncCount = 0;

        // 2. Set the custom software frame rate limit
        Application.targetFrameRate = targetFPS;
    }
}
