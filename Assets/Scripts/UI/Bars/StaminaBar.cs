using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private PlayerStaminaController staminaController;

    [Header("Stamina UI Elements")]
    [SerializeField] private Image staminaProgressUI = null;
    [SerializeField] private CanvasGroup staminaCanvasGroup = null;
    [SerializeField] private bool hideWhenNotDrainingStamina = false;

    [SerializeField] private float duration = 1.0f;

    private Coroutine canvasGroupCoroutine;

    private void Start()
    {
        staminaController = FindAnyObjectByType<PlayerStaminaController>();

        staminaController.onStaminaChange += UpdateStamina;
    }

    void UpdateStamina(float value)
    {
        staminaProgressUI.fillAmount = value;
    }

    private bool previousHasRegenerated;

    private void Update()
    {
        bool hasRegenerated = PlayerStaminaController.i.hasRegenerated;

        if (hasRegenerated != previousHasRegenerated)
        {
            previousHasRegenerated = hasRegenerated;

            if (canvasGroupCoroutine != null)
            {
                StopCoroutine(canvasGroupCoroutine);
            }

            canvasGroupCoroutine = StartCoroutine(
                FadeCanvasGroup(hasRegenerated ? 0f : 1f)
            );
        }
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        if (!hideWhenNotDrainingStamina)
            yield break;

        float startAlpha = staminaCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;
            staminaCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        staminaCanvasGroup.alpha = targetAlpha;
    }
}
