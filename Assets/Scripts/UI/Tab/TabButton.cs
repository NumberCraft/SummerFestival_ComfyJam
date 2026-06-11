using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;

    public Image background;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    [SerializeField] private Animator animator;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    private void Awake()
    {
        /*tabGroup = GetComponentInParent<TabGroup>();

        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.Log($"Tab button - {gameObject.name} - doesn't have any animator.");

        background = GetComponent<Image>();
        tabGroup.Subscribe(this);*/
    }

    public void Select()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Deselect");
            animator.ResetTrigger("Select");
            animator.SetTrigger("Select");
        }
        if(onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Select");
            animator.ResetTrigger("Deselect");
            animator.SetTrigger("Deselect");
        }
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
    }

    private void OnValidate()
    {
        if (TryGetComponent(out Image bg))
        {
            background = bg;
        }

        if (TryGetComponent(out Animator anim))
        {
            animator = anim;
        }
    }
}
