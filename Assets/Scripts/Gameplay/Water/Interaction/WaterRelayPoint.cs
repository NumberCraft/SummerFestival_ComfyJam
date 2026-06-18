using UnityEngine;
using UnityEngine.UI;

public class WaterRelayPoint : WaterSource, IWaterable
{
    [SerializeField] private float maxWaterAmount;
    private float currentWaterAmount;
    private bool isAvailable;

    [Header("UI Meter")]
    [SerializeField] private GameObject meterCanvas;   // world space canvas child
    [SerializeField] private Image meterFill;          // the fill image inside it

    public override void Update()
    {
        if (currentWaterAmount >= maxWaterAmount)
        {
            isAvailable = true;
        }

        if (!isAvailable)
            return;

        base.Update();
    }

    public void Water(float waterAmount)
    {
        currentWaterAmount += waterAmount;

        if (meterCanvas != null)
        {
            meterCanvas.SetActive(true);

            if (meterFill != null)
                meterFill.fillAmount = Mathf.Clamp01(currentWaterAmount);
        }

        if (currentWaterAmount >= maxWaterAmount)
            FullyWatered();
    }

    public void FullyWatered()
    {
        if (meterCanvas != null)
            meterCanvas.SetActive(false);
    }

    public bool IsFullyWatered()
    {
        return currentWaterAmount >= maxWaterAmount;
    }
}
