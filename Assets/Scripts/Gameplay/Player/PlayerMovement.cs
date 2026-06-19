using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPausable
{
    #region Properties

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float airSpeed;
    private float moveSpeed;

    [SerializeField] private float groundDrag;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private bool readyToJump;

    [SerializeField] private float landingThreshold = 1.0f; // Distance to start landing anim

    [Header("Particle System")]
    [SerializeField] private ParticleSystem speedLines;

    [SerializeField] private float minDownVelForSpeedLines = -2f;

    [Header("Keybinds")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask ground;

    public bool grounded { get; private set; }
    private bool wasGrounded;

    [Header("Slope Handling")]
    [SerializeField] private float minSlopeAngle = 20f;
    [SerializeField] private float maxSlopeAngle = 60f;

    [SerializeField] private float slopeVerticalVelocityTreshold = 0.05f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider coll;
    [HideInInspector] public PlayerStaminaController staminaController;
    private AnimationStateController animationStateController;

    [Header("Audio")]
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private float sprintPitch = 1.2f;
    [SerializeField] private float walkPitch = 1f;

    [SerializeField] private AudioSource jumpAudioSource;

    [SerializeField] private float audioFadeDuration = 1f;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    public MovementState state;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        coll = GetComponentInChildren<CapsuleCollider>();

        staminaController = GetComponent<PlayerStaminaController>();
        animationStateController = GetComponent<AnimationStateController>();

        if (speedLines.isPlaying)
            speedLines.Stop();
    }

    private void Update()
    {
        // Standard ground check for physics
        grounded = Physics.Raycast(transform.position + offset, Vector3.down, playerHeight * 0.5f + 0.1f, ground);

        if (!grounded && rb.linearVelocity.y < 0) // Only check while falling
        {
            // Shoot a raycast further down to "see" the floor coming
            if (Physics.Raycast(transform.position + offset, Vector3.down, playerHeight * 0.5f + landingThreshold, ground))
            {
                animationStateController.TriggerLand();
            }
        }

        // Existing landing logic for the actual physics state
        if (grounded && !wasGrounded)
        {
            // Optional: Trigger a "Hard Land" or Camera Shake here
        }
        wasGrounded = grounded;

        MovementStateControl();
        MyInput();
        SpeedControl();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0f;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            staminaController.StaminaJump();

            //Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovementStateControl()
    {
        if (grounded && Input.GetKey(sprintKey))
        {
            if (moveDirection != Vector3.zero)
            {
                if (staminaController.playerStamina > 0)
                {
                    staminaController.Sprinting();

                    moveSpeed = sprintSpeed;
                    state = MovementState.run;

                    if (!speedLines.isPlaying)
                        speedLines.Play();

                    AudioManager.Play("run", walkAudioSource, sprintPitch);
                }
                else
                {
                    moveSpeed = walkSpeed;
                    state = MovementState.walk;

                    if (!speedLines.isPlaying)
                        speedLines.Play();

                    AudioManager.Play("run", walkAudioSource, walkPitch);
                }
            }
            else
            {
                staminaController.weAreSprinting = false;
                state = MovementState.standing;

                if (speedLines.isPlaying)
                    speedLines.Stop();

                //StartCoroutine(FadeOut(walkAudioSource, audioFadeOutDuration));
                //AudioManager.FadeStop(walkAudioSource, audioFadeOutDuration);
                AudioManager.i.StartCoroutine(AudioManager.i.FadeOut(walkAudioSource, audioFadeDuration));
            }
        }
        else if (grounded)
        {
            moveSpeed = walkSpeed;
            staminaController.weAreSprinting = false;

            if (moveDirection != Vector3.zero)
            {
                state = MovementState.walk;

                AudioManager.Play("run", walkAudioSource, walkPitch);
            }
            else
            {
                state = MovementState.standing;

                //StartCoroutine(FadeOut(walkAudioSource, audioFadeOutDuration));
                //AudioManager.FadeStop(walkAudioSource, audioFadeOutDuration);
                AudioManager.i.StartCoroutine(AudioManager.i.FadeOut(walkAudioSource, audioFadeDuration));
            }

            if (speedLines.isPlaying)
                speedLines.Stop();
        }
        else
        {
            moveSpeed = airSpeed;
            staminaController.weAreSprinting = false;
            state = MovementState.air;

            if (rb.linearVelocity.y < minDownVelForSpeedLines)
            {
                if (!speedLines.isPlaying)
                    speedLines.Play();
            }
            else
            {
                if (speedLines.isPlaying)
                    speedLines.Stop();
            }

            //StartCoroutine(FadeOut(walkAudioSource, audioFadeOutDuration));
            //AudioManager.FadeStop(walkAudioSource, audioFadeOutDuration);
            AudioManager.i.StartCoroutine(AudioManager.i.FadeOut(walkAudioSource, audioFadeDuration));
        }
    }

    #region Move
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > slopeVerticalVelocityTreshold)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (grounded && Input.GetKey(sprintKey))
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    public void Jump()
    {
        exitingSlope = true;
        readyToJump = false;

        rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0f, rb.angularVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        animationStateController.TriggerJump();

        AudioManager.Play("jump", jumpAudioSource);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }
    #endregion

    #region Speed Control

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    #endregion

    #region Slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position + offset, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle > minSlopeAngle;
        }

        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    #endregion

    public void Pause()
    {
        this.enabled = false;
    }

    public void Continue()
    {
        this.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + offset, Vector3.down * playerHeight * 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + offset, Vector3.down * (playerHeight * 0.5f + landingThreshold));

        Gizmos.color = Color.green;

        if (OnSlope() && !exitingSlope)
        {
            Gizmos.DrawRay(transform.position + offset, GetSlopeMoveDirection() * moveSpeed * 20f);
        }
    }
}

public enum MovementState
{
    standing,
    walk,
    run,
    air
}