using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerBody;

    [SerializeField] private Transform camTransform;

    [SerializeField] private CinemachineStateDrivenCamera cinemachineStateDrivenCamera;

    [Header("Properties")]
    [SerializeField] private float sens = 200f;
    [SerializeField] private float rotationSpeed;

    [HideInInspector] public bool isOrientationFixed = false;

    private void Start()
    {
        cinemachineStateDrivenCamera = GetComponentInChildren<CinemachineStateDrivenCamera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        if (!isOrientationFixed)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 viewDir = orientation.position - new Vector3(camTransform.position.x, orientation.position.y, camTransform.position.z);
            orientation.forward = viewDir.normalized;

            //Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            Vector3 direction = inputDir.normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion smoothedRotation = Quaternion.Slerp(playerBody.rotation, targetRotation, Time.deltaTime * rotationSpeed); ;
                playerBody.rotation = smoothedRotation;
            }
        }
        else
        {
            orientation.localRotation = Quaternion.Euler(Vector3.zero);

            playerBody.rotation = orientation.rotation;
        }
    }
}
