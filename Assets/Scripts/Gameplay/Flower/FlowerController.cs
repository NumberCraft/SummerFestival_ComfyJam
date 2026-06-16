using UnityEngine;
using UnityEngine.UI;

public class FlowerController : MonoBehaviour, IWaterable
{
    [SerializeField] private Transform mainTransform;

    [Header("Growth (Placeholder)")]
    [SerializeField] private Vector3 grownScale = new Vector3(1f, 2f, 1f);
    private Vector3 startScale;

    public bool isFullyWatered { get; private set; }

    private float waterProgress = 0f; // 0 to 1
    private WaterGun waterGun;

    private void Start()
    {
        waterGun = FindAnyObjectByType<WaterGun>();

        startScale = mainTransform.localScale;
    }

    public void Water(float waterAmount)
    {
        if (isFullyWatered) return;

        waterProgress += waterAmount;
        waterProgress = Mathf.Clamp01(waterProgress);

        mainTransform.localScale = Vector3.Lerp(startScale, grownScale, waterProgress);

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

        // Placeholder growth - scale up the cube
        mainTransform.localScale = grownScale;

        // Tell gun to stop the stream
        waterGun?.OnFlowerFullyWatered();

        Debug.Log(gameObject.name + " fully watered and grown!");
    }

    public bool IsFullyWatered()
    {
        return isFullyWatered;
    }
}