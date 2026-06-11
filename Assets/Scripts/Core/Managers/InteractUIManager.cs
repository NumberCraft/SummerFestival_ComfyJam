using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractUIManager : MonoBehaviour
{
    public static InteractUIManager Instance;

    [Header("Dialogue")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("Item")]
    [SerializeField] private TextMeshProUGUI itemText;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(InteractType type)
    {
        //Debug.Log($"Showing {type} ui.");

        switch (type)
        {
            case InteractType.Dialogue:
                dialogueText.gameObject.SetActive(true); break;
            case InteractType.Item:
                itemText.gameObject.SetActive(true); break;
        }
    }

    public void ShowAndSet(InteractType type, string text)
    {
        //Debug.Log($"Showing and setting {type} ui.");

        switch (type)
        {
            case InteractType.Dialogue:
                dialogueText.gameObject.SetActive(true);
                dialogueText.text = text;
                break;
            case InteractType.Item:
                itemText.gameObject.SetActive(true);
                itemText.text = text;
                break;
        }
    }

    public void Set(InteractType type, string text)
    {
        //Debug.Log($"Showing and setting {type} ui.");

        switch (type)
        {
            case InteractType.Dialogue:
                dialogueText.text = text;
                break;
            case InteractType.Item:
                itemText.text = text;
                break;
        }
    }

    public void Hide(InteractType type)
    {
        //Debug.Log($"Hiding {type} ui.");

        switch (type)
        {
            case InteractType.Dialogue:
                dialogueText.gameObject.SetActive(false); break;
            case InteractType.Item:
                itemText.gameObject.SetActive(false); break;
        }
    }

    public void HideAll()
    {
        dialogueText.gameObject.SetActive(false);
        itemText.gameObject.SetActive(false);
    }
}

public enum InteractType
{
    Dialogue,
    Item,
}