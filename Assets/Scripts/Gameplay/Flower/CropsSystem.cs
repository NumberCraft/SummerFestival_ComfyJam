using System.Collections.Generic;
using UnityEngine;

public class CropsSystem : MonoBehaviour
{
    [SerializeField] private List<CropController> crops = new();

    [SerializeField] private Animator anim;

    [SerializeField] private DialogueTrigger dialogueTrigger;

    [SerializeField] private List<Animator> animators = new();

    [SerializeField] private Collider coll;

    [SerializeField] private int wateredCrops = 0;

    private bool isOpened = false;

    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        foreach (var target in crops)
        {
            target.cropsSystem = this;
        }
    }

    private void Update()
    {
        if (wateredCrops >= crops.Count)
        {
            if (!isOpened)
            {
                isOpened = true;

                dialogueTrigger.ChangeDialogueIndexTo(2);

                foreach (var animator in animators)
                {
                    animator.SetBool("isCelebrating", true);
                }

                coll.enabled = false;

                AudioManager.Play("success");
            }
        }
    }

    public void AddShottedTarget()
    {
        wateredCrops++;
    }
}
