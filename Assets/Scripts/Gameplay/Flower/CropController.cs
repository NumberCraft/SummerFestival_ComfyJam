using UnityEngine;
using UnityEngine.UI;

public class CropController : MonoBehaviour, IWaterable
{
    [Header("Growth (Placeholder)")]
    [SerializeField] private Vector3 grownScale = new Vector3(1f, 2f, 1f);
    private Vector3 startScale;


    [Header("UI Meter")]
    [SerializeField] private GameObject meterCanvas;   // world space canvas child
    [SerializeField] private Image meterFill;          // the fill image inside it

    public bool isFullyWatered { get; private set; }

    private float waterProgress = 0f; // 0 to 1
    private WaterGun waterGun;

    public CropsSystem cropsSystem { get; set; }

    private void Start()
    {
        waterGun = FindAnyObjectByType<WaterGun>();

        // Hide meter at start
        if (meterCanvas != null)
            meterCanvas.SetActive(false);

        startScale = transform.localScale;
    }

    public void Water(float waterAmount)
    {
        if (isFullyWatered) return;

        waterProgress += waterAmount;
        waterProgress = Mathf.Clamp01(waterProgress);

        transform.localScale = Vector3.Lerp(startScale, grownScale, waterProgress);

        // Show and update meter
        if (meterCanvas != null)
        {
            meterCanvas.SetActive(true);

            if (meterFill != null)
                meterFill.fillAmount = waterProgress;
        }

        if (waterProgress >= 1f)
            FullyWatered();
    }

    public void FullyWatered()
    {
        isFullyWatered = true;

        // Placeholder growth - scale up the cube
        transform.localScale = grownScale;

        // Hide meter
        if (meterCanvas != null)
            meterCanvas.SetActive(false);

        // Tell gun to stop the stream
        waterGun?.OnFlowerFullyWatered();

        cropsSystem.AddShottedTarget();

        Debug.Log(gameObject.name + " fully watered and grown!");
    }

    public bool IsFullyWatered()
    {
        return isFullyWatered;
    }
}
