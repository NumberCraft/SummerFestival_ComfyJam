using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private float speed = 0.05f;

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
    }
}
