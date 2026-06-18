using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaterIndicator : MonoBehaviour
{
    private WaterCollector waterCollector;

    [Header("Stamina UI Elements")]
    [SerializeField] private Image waterIndicatorProgressUI = null;
    [SerializeField] private CanvasGroup waterIndicatorCanvasGroup = null;
    [SerializeField] private bool hideWhenNotConnected = false;

    [SerializeField] private float duration = 1.0f;

    private Coroutine canvasGroupCoroutine;

    private void Start()
    {
        waterCollector = FindAnyObjectByType<WaterCollector>();

        waterCollector.onDistanceChange += UpdateIndicator;
        waterCollector.onConnect += ShowIndicator;
        waterCollector.onDisconnect += HideIndicator;
    }

    void UpdateIndicator(float value)
    {
        waterIndicatorProgressUI.fillAmount = Mathf.Clamp01(value);
    }

    void HideIndicator(WaterSource waterSource)
    {
        canvasGroupCoroutine = StartCoroutine(FadeCanvasGroup(0f));
    }

    void ShowIndicator(WaterSource waterSource)
    {
        canvasGroupCoroutine = StartCoroutine(FadeCanvasGroup(1f));
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        if (!hideWhenNotConnected)
            yield break;

        float startAlpha = waterIndicatorCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;
            waterIndicatorCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        waterIndicatorCanvasGroup.alpha = targetAlpha;
    }
}
