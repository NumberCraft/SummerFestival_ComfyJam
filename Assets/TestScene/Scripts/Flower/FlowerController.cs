using UnityEngine;
using UnityEngine.UI;

public class FlowerController : MonoBehaviour
{
    [Header("Watering Settings")]
    [SerializeField] private float wateringRate = 0.4f; // meter fill per second

    [Header("Growth (Placeholder)")]
    [SerializeField] private Vector3 grownScale = new Vector3(1f, 2f, 1f);

    [Header("UI Meter")]
    [SerializeField] private GameObject meterCanvas;   // world space canvas child
    [SerializeField] private Image meterFill;          // the fill image inside it

    public bool isFullyWatered { get; private set; }

    private float waterProgress = 0f; // 0 to 1
    private WaterGun waterGun;

    private void Start()
    {
        waterGun = FindAnyObjectByType<WaterGun>();

        // Hide meter at start
        if (meterCanvas != null)
            meterCanvas.SetActive(false);
    }

    private void Update()
    {
        // Keep meter facing camera
        if (meterCanvas != null && meterCanvas.activeSelf && Camera.main != null)
        {
            meterCanvas.transform.LookAt(Camera.main.transform);
            meterCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    // Called every frame by WaterGun while shooting this flower
    public void AddWater(float deltaTime)
    {
        if (isFullyWatered) return;

        waterProgress += wateringRate * deltaTime;
        waterProgress = Mathf.Clamp01(waterProgress);

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
    public void AddWaterAmount(float amount)
    {
        if (isFullyWatered) return;

        waterProgress += amount;
        waterProgress = Mathf.Clamp01(waterProgress);

        // Show meter
        if (meterCanvas != null)
        {
            meterCanvas.SetActive(true);

            if (meterFill != null)
                meterFill.fillAmount = waterProgress;
        }

        if (waterProgress >= 1f)
            FullyWatered();
    }


    private void FullyWatered()
    {
        isFullyWatered = true;

        // Placeholder growth - scale up the cube
        transform.localScale = grownScale;

        // Hide meter
        if (meterCanvas != null)
            meterCanvas.SetActive(false);

        // Tell gun to stop the stream
        waterGun?.OnFlowerFullyWatered();

        Debug.Log(gameObject.name + " fully watered and grown!");
    }
}