using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Properties")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private ItemScriptableObject keyItem;

    [Header("Interact Properties")]
    [SerializeField] private float openDistance = 5f;
    [SerializeField] private LayerMask playerLayer;
    //[SerializeField] private LayerMask openMask;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    public bool isOpen { get; private set; }

    private Animator anim;

    private RaycastHit hit;

    private void Start()
    {
        anim = GetComponent<Animator>();

        GameObject audioSource = new GameObject("AudioSource");
        audioSource.transform.SetParent(transform, false);
        
        this.audioSource = audioSource.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (isLocked)
        {
            if (PlayerInventory.i.items.Contains(keyItem))
            {
                isLocked = false;
            }
        }
        if (Physics.CheckSphere(transform.position, openDistance, playerLayer))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, openDistance, playerLayer);

            if (colliders.Length > 0)
            {
                if (!isOpen)
                {
                    InteractUIManager.Instance.ShowAndSet(InteractType.Door, "Press 'E' To Open The Door");
                }
                else
                {
                    InteractUIManager.Instance.ShowAndSet(InteractType.Door, "Press 'E' To Close The Door");
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!isLocked)
                    {
                        Open();
                    }
                }
            }
        }
        else
        {
            InteractUIManager.Instance.Hide(InteractType.Door);
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;

            anim.SetTrigger("Open");

            //AudioManager.Play("DoorOpen", audioSource);
        }
        else
        {
            isOpen = false;

            anim.SetTrigger("Close");

            //AudioManager.Play("DoorClose", audioSource);
        }
    }
}
