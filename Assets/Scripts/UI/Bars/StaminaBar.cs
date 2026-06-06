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

    private void Start()
    {
        staminaController = FindAnyObjectByType<PlayerStaminaController>();

        staminaController.onStaminaChange += UpdateStamina;
    }

    void UpdateStamina(float value)
    {
        staminaProgressUI.fillAmount = value;

        StartCoroutine(SetCanvasGroupAlpha(value));
    }

    private IEnumerator SetCanvasGroupAlpha(float value)
    {
        if (!hideWhenNotDrainingStamina)
        {
            yield break;
        }

        staminaCanvasGroup.alpha = value;

        yield return null;
    }
}
