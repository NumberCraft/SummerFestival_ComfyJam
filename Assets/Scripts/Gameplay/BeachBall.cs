using System.Collections.Generic;
using UnityEngine;

public class BeachBall : MonoBehaviour, IBlobHitable
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private DialogueTrigger dialogueTrigger;

    [SerializeField] private List<Animator> animators = new();

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    [ContextMenu("Hit")]
    public void Hit()
    {
        rb.isKinematic = false;

        dialogueTrigger.ChangeDialogueIndexTo(2);

        foreach (var animator in animators)
        {
            animator.SetBool("isCelebrating", true);
        }
    }
}
