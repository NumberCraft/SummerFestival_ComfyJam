using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DialogueTrigger : MonoBehaviour
{
    #region Properties
    [Header("MainProperties")]
    [SerializeField] private string Name;

    [Space(20)]

    [SerializeField] private LayerMask playerLayer;

    [Space(20)]

    [SerializeField] private Transform target;
    
    [Space(20)]

    [SerializeField] private Vector3 offset;
    [SerializeField] private float enterDialogueRange = 8f;

    [Space(20)]

    [SerializeField] private int maxDialogCount = 1;
    [SerializeField] private int dialogIndex = 1;

    [Space(20)]

    [SerializeField] private bool canGetOutOfDialogWhenNotNear = true;

    [Space(20)]

    [SerializeField] private bool canStartAgain = true;

    public Action onDialogStart;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private MultiAimConstraint headAim;
    [SerializeField] private Transform headAimTarget;
    [SerializeField] private bool canLookAtThePlayer = true;

    public static int currentDialogCount;

    #endregion

    private void Update()
    {
        if (Physics.CheckSphere(transform.position + offset, enterDialogueRange, playerLayer) && !DialogueSystem.i.dialogueIsPlaying)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + offset, enterDialogueRange);

            if (colliders.Length > 0)
            {
                //if (canLookAtThePlayer)
                //    headAimTarget.position = Camera.main.transform.position;

                InteractUIManager.Instance.Show(InteractType.Dialogue);

                //enterDialogueText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    DialogueSystem.i.EnterDialogueMode(Name + "_" + dialogIndex, animator, headAim, canLookAtThePlayer, target);

                    onDialogStart?.Invoke();

                    if (dialogIndex < currentDialogCount)
                    {
                        dialogIndex++;
                        currentDialogCount++;
                    }
                }
            }
        }
        else if (!Physics.CheckSphere(transform.position + offset, enterDialogueRange, playerLayer) && DialogueSystem.i.dialogueIsPlaying)
        {
            if (canGetOutOfDialogWhenNotNear)
            {
                DialogueSystem.i.StartCoroutine(DialogueSystem.i.ExitDialogueModeCoroutine());
            }
        }
        else if (!Physics.CheckSphere(transform.position + offset, enterDialogueRange, playerLayer) && !DialogueSystem.i.dialogueIsPlaying)
        {
            if (!IsNearToOther())
            {
                InteractUIManager.Instance.Hide(InteractType.Dialogue);
            }       
        }

        if (DialogueSystem.i.dialogueIsPlaying)
        {
            InteractUIManager.Instance.Hide(InteractType.Dialogue);
        }
    }

    private bool IsNearToOther()
    {
        DialogueTrigger[] dialogueTriggers = FindObjectsByType<DialogueTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var dt in dialogueTriggers)
        {
            if (Physics.CheckSphere(dt.transform.position + offset, enterDialogueRange, playerLayer))
            {
                Collider[] colliders = Physics.OverlapSphere(dt.transform.position + offset, enterDialogueRange);

                if (colliders.Length > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + offset, enterDialogueRange);
    }
}
