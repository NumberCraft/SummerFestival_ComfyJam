using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObject/DialogueAudioInfoSO", order = 2)]
public class DialogueAudioInfoSO : ScriptableObject
{
    public string id;
    public string soundName;
    public bool stopAudioSource;
    [Range(1, 10)]
    public int frequency = 3;
    [Range(-3, 3)]
    public float minPitch = 0.7f;
    [Range(-3, 3)]
    public float maxPitch = 1.5f;
}
