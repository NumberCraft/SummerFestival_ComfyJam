using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEvent : MonoBehaviour
{
    [SerializeField] private List<UnityEvent> dialogueEvents;

    public void EventStart(int index)
    {
        if (dialogueEvents[index] != null)
        {
            dialogueEvents[index]?.Invoke();
        }
    }
}
