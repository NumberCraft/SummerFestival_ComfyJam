using UnityEngine;

public class SecurityBooth : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private bool isOpened;

    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    public void Open()
    {
        if (isOpened) return;

        isOpened = true;

        anim.SetBool("isOpened", true);
    }
}
