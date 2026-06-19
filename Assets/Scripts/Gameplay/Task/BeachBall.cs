using System.Collections.Generic;
using UnityEngine;

public class BeachBall : MonoBehaviour, IBlobHitable
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private DialogueTrigger dialogueTrigger;

    [SerializeField] private List<Animator> animators = new();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private bool isHitted;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    [ContextMenu("Hit")]
    public void Hit()
    {
        if (isHitted)
        {
            return;
        }

        isHitted = true;

        rb.isKinematic = false;

        dialogueTrigger.ChangeDialogueIndexTo(2);

        foreach (var animator in animators)
        {
            animator.SetBool("isCelebrating", true);
        }

        AudioManager.Play("success");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play sound when the ball hits something
        //audioSource.Play();
        AudioManager.Play("bounce", audioSource);
    }
}
