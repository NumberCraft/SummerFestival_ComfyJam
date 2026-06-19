using System.Collections.Generic;
using UnityEngine;

public class Fountain : MonoBehaviour, IWaterable
{
    [SerializeField] private Animator anim;

    [SerializeField] private DialogueTrigger dialogueTrigger;

    [SerializeField] private List<Animator> animators = new();

    public bool isFullyWatered { get; private set; }

    private float waterProgress = 0f; // 0 to 1
    private WaterGun waterGun;

    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        waterGun = FindAnyObjectByType<WaterGun>();
    }

    public void Water(float waterAmount)
    {
        if (isFullyWatered) return;

        waterProgress += waterAmount;
        waterProgress = Mathf.Clamp01(waterProgress);

        anim.SetFloat("fillAmount", waterProgress);

        if (waterProgress >= 1f)
            FullyWatered();
    }
    public void AddWaterAmount(float amount)
    {
        if (isFullyWatered) return;

        waterProgress += amount;
        waterProgress = Mathf.Clamp01(waterProgress);

        if (waterProgress >= 1f)
            FullyWatered();
    }

    public void FullyWatered()
    {
        isFullyWatered = true;

        anim.SetFloat("fillAmount", 1f);

        // Tell gun to stop the stream
        waterGun?.OnFlowerFullyWatered();

        dialogueTrigger.ChangeDialogueIndexTo(2);

        foreach (var animator in animators)
        {
            animator.SetBool("isCelebrating", true);
        }

        AudioManager.Play("success");

        Debug.Log(gameObject.name + " fully watered and grown!");
    }

    public bool IsFullyWatered()
    {
        return isFullyWatered;
    }
}
