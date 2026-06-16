using UnityEngine;

public class WaterRelayPoint : WaterSource, IWaterable
{
    [SerializeField] private float maxWaterAmount;
    private float currentWaterAmount;
    private bool isAvailable;

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
    }

    public void FullyWatered()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFullyWatered()
    {
        throw new System.NotImplementedException();
    }
}
