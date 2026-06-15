using System.Collections.Generic;
using UnityEngine;

public class DamSystem : MonoBehaviour
{
    [SerializeField] private List<Target> targets = new();

    [SerializeField] private Animator anim;

    [SerializeField] private WaterSource waterSource;

    private int shottedTargetCount = 0;

    private bool isOpened = false;

    private void Start()
    {
        if (waterSource == null)
            waterSource = GetComponentInChildren<WaterSource>();

        if (anim == null)
            anim = GetComponent<Animator>();

        waterSource.enabled = false;

        foreach (var target in targets)
        {
            target.dam = this;
        }
    }

    private void Update()
    {
        if (shottedTargetCount >= targets.Count)
        {
            if (!isOpened)
            {
                isOpened = true;

                anim.SetBool("isOpened", isOpened);

                waterSource.enabled = true;
            }
        }
    }

    public void AddShottedTarget()
    {
        shottedTargetCount++;
    }
}
