using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaController : MonoBehaviour
{
    [Header("Stamina Main Parameters")]
    public float playerStamina = 100f;
    [SerializeField] private float maxStamina = 100f;

    [Space(20)]

    [SerializeField] private float jumpCost = 20f;

    [Space(20)]

    [HideInInspector] public bool hasRegenerated = true;
    [HideInInspector] public bool weAreSprinting = false;

    [Space(20)]

    [Header("Stamina Regen Parameters")]
    [Range(0, 50)][SerializeField] private float staminaDrain = 0.5f;
    [Range(0, 50)][SerializeField] private float staminaRegen = 0.5f;

    public Action<float> onStaminaChange;

    private PlayerMovement playerMovement;

    public static PlayerStaminaController i;

    private void Start()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!weAreSprinting)
        {
            if (playerStamina <= maxStamina)
            {
                playerStamina += staminaRegen * Time.deltaTime;
                UpdateStamina(1);

                if (playerStamina >= maxStamina)
                {
                    hasRegenerated = true;
                }
            }
        }
    }

    public void Sprinting()
    {
        if (playerStamina > 0)
        {
            weAreSprinting = true;
            playerStamina -= staminaDrain * Time.deltaTime;
            UpdateStamina(1);

            hasRegenerated = false;

            /*if (playerStamina <= 0)
            {
                hasRegenerated = false;
            }*/
        }
    }

    public void StaminaJump()
    {
        if (playerStamina >= jumpCost)
        {
            playerStamina -= jumpCost;
            playerMovement.Jump();
            UpdateStamina(1);

            hasRegenerated = false;
        }
    }

    void UpdateStamina(int value)
    {
        onStaminaChange?.Invoke(playerStamina / maxStamina);
    }
}
