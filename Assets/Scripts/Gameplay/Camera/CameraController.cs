using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour, IPausable
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerBody;

    [SerializeField] private Transform camTransform;

    [SerializeField] private CinemachineStateDrivenCamera cinemachineStateDrivenCamera;
    [SerializeField] private List<CinemachineCamera> cinemachineCameras;

    [Header("Properties")]
    [SerializeField] private float sens = 200f;
    [SerializeField] private float rotationSpeed;
    [HideInInspector] public bool isOrientationFixed = false;

    [Header("Zoom")]
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] float minRadius = 2f;
    [SerializeField] float maxRadius = 15f;

    private void Start()
    {
        cinemachineStateDrivenCamera = GetComponentInChildren<CinemachineStateDrivenCamera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Rotate();
        Zoom();
    }

    private Coroutine rotateCoroutine;

    private void Rotate()
    {
        if (!isOrientationFixed)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 viewDir = orientation.position -
                              new Vector3(
                                  camTransform.position.x,
                                  orientation.position.y,
                                  camTransform.position.z);

            orientation.forward = viewDir.normalized;

            Vector3 inputDir =
                orientation.forward * verticalInput +
                orientation.right * horizontalInput;

            if (inputDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(inputDir.normalized);

                playerBody.rotation = Quaternion.Slerp(
                    playerBody.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (rotateCoroutine != null)
                    StopCoroutine(rotateCoroutine);

                rotateCoroutine = StartCoroutine(
                    RotateTo(orientation.forward));
            }
        }
        else
        {
            orientation.localRotation = Quaternion.identity;
            playerBody.rotation = orientation.rotation;
        }
    }

    private IEnumerator RotateTo(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        while (Quaternion.Angle(playerBody.rotation, targetRotation) > 0.1f)
        {
            playerBody.rotation = Quaternion.RotateTowards(
                playerBody.rotation,
                targetRotation,
                rotationSpeed * 100f * Time.deltaTime);

            yield return null;
        }

        playerBody.rotation = targetRotation;
    }

    private void Zoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            foreach (var cam in cinemachineCameras)
            {
                /*CinemachineInputAxisController axisController = cam.GetComponent<CinemachineInputAxisController>();

                axisController.Controllers[0].Input.Gain =
                Mathf.Clamp(axisController.Controllers[0].Input.Gain +
                    scroll * zoomSpeed, minScale, maxScale);*/

                CinemachineOrbitalFollow orbitalFollow = cam.GetComponent<CinemachineOrbitalFollow>();

                orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius - scroll * zoomSpeed, minRadius, maxRadius);
            }
        }
    }

    public void Pause()
    {
        foreach (var cam in cinemachineCameras)
        {
            cam.GetComponent<CinemachineInputAxisController>().enabled = false;
        }

        this.enabled = false;
    }

    public void Continue()
    {
        foreach (var cam in cinemachineCameras)
        {
            cam.GetComponent<CinemachineInputAxisController>().enabled = true;
        }

        this.enabled = true;
    }
}
