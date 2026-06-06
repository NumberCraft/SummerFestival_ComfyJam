using System.Collections;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    float speed = 0.0f;

    [SerializeField] private float accelaration = 2.0f;
    [SerializeField] private float decelaration = 2.0f;
    [SerializeField] private float maximumWalkSpeed = 0.5f;
    [SerializeField] private float maximumRunSpeed = 2.0f;

    [HideInInspector] public PlayerMovement playerMovement;
    [SerializeField] private Rigidbody rb;

    private bool isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        float currentMaxSpeed = Input.GetKey(KeyCode.LeftShift) ? maximumRunSpeed : maximumWalkSpeed;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) && speed < currentMaxSpeed)
        {
            speed += Time.deltaTime * accelaration;
        }


        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && speed > 0.0f)
        {
            speed -= Time.deltaTime * decelaration;
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && speed != 0.0f && speed < 0.05f)
        {
            speed = 0.0f;
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift) && speed > currentMaxSpeed)
        {
            speed = currentMaxSpeed;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) && speed > currentMaxSpeed)
        {
            speed -= Time.deltaTime * decelaration;

            if (speed > currentMaxSpeed && speed < (currentMaxSpeed + 0.05f))
            {
                speed = currentMaxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) && speed < currentMaxSpeed && speed > (currentMaxSpeed - 0.05f))
        {
            speed = currentMaxSpeed;
        }

        animator.SetFloat("Speed", speed);

        // 1. Tell animator if we are on the ground
        animator.SetBool("isGrounded", playerMovement.grounded);

        // 2. Feed the vertical velocity to the animator
        // Positive = Jumping up, Negative = Falling down
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    // Call this from the Movement script when jumping
    public void TriggerJump()
    {
        animator.SetTrigger("jump");
    }

    // Call this from the Movement script when landing
    public void TriggerLand()
    {
        animator.SetTrigger("land");
    }

    private void OnDisable()
    {
        speed = 0f;

        animator.SetFloat("Speed", speed);
    }
}
