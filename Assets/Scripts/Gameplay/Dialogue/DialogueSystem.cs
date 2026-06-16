using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    #region Properties
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanelPrefab;

    private GameObject dialoguePanel;
    private Transform target;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI displayNameText;

    [Space(20)]

    [SerializeField] private float typingSpeed = 0.04f;

    private List<LevelDialog> levelDialogList;
    private LevelDialog currentLevelDialog;

    public bool dialogueIsPlaying { get; private set; }
    public Action onDialogEnd { get; set; }

    private bool canContinueToNextLine;

    private Coroutine displayLineCoroutine;

    private const string Speaker_Tag = "speaker";
    private const string Audio_Tag = "audio";
    private const string Action_Tag = "action";
    //private const string Portrait_Tag = "portrait";
    //private const string Layout_Tag = "layout";

    [Header("Animation")]
    [SerializeField] private float animationTransitionSpeed;
    [SerializeField] private float animationThreshold = 0.015f;
    private Animator anim;

    private MultiAimConstraint headAim;
    private bool canLookAtThePlayer;

    private string actionId;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    [SerializeField] private bool makePredictable;

    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary = new();

    public DialogueTrigger currentDialogueTrigger { get; private set; }

    public static DialogueSystem i { get; private set; }
    #endregion

    private void Awake()
    {
        i = this;

        currentAudioInfo = defaultAudioInfo;

        InitializeAudioInfoDictionary();
    }

    private void Start()
    {
        LoadLevelData();

        dialogueIsPlaying = false;
    }

    private void Update()
    {
        if (currentLevelDialog == null || !dialogueIsPlaying || !canContinueToNextLine)
            return;

        if (InputManager.i.GetSubmitPressed())
        {
            if (currentLevelDialog.currentDialog.currentChoices.Count != 0)
                return;

            ContinueStory();
        }
    }

    private void ContinueStory()
    {
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }

        if (currentLevelDialog.canContinue)
        {
            string nextLine = currentLevelDialog.Continue();
            // handle case where the last line is an external function
            if (nextLine.Equals("") && !currentLevelDialog.canContinue)
            {
                StartCoroutine(ExitDialogueModeCoroutine());
            }
            // otherwise, handle the normal case for continuing the story
            else
            {
                // handle tags
                HandleTags(currentLevelDialog.currentDialog.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else
        {
            StartCoroutine(ExitDialogueModeCoroutine());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        
        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        //StartCoroutine(PlayAnimation());

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (InputManager.i.GetSubmitPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else
            {
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);

                dialogueText.maxVisibleCharacters++;

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        canContinueToNextLine = true;
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        string soundName = currentAudioInfo.soundName;
        int frequency = currentAudioInfo.frequency;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;

        if (currentDisplayedCharacterCount % frequency == 0)
        {
            if (stopAudioSource)
            {
                AudioManager.Stop(soundName);
            }

            if (makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();

                //int predictableIndex = hashCode % AudioManager.GetSoundArray(soundName).Length;
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;

                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;

                    AudioManager.Play(soundName, predictablePitch);
                }
                else
                {
                    AudioManager.Play(soundName, minPitch);
                }
            }
            else
            {
                AudioManager.Play(soundName, UnityEngine.Random.Range(minPitch, maxPitch));
            }
        }
    }

    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>
        {
            { defaultAudioInfo.id, defaultAudioInfo }
        };
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }

    private void SetCurrentAudioInfo(string id)
    {
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            this.currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning($"Failed to find audio info for id: {id}");
        }
    }

    private void SetCurrentAction(string id)
    {
        actionId = id;

        anim.SetTrigger(actionId);
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length < 2)
            {
                //Debug.LogError("Tag could not be appropriately parsed: " + tag);
                Debug.LogWarning("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case Speaker_Tag:
                    displayNameText.text = tagValue;
                    break;
                case Audio_Tag:
                    SetCurrentAudioInfo(tagValue);
                    break;
                case Action_Tag:
                    SetCurrentAction(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    public void EnterDialogueMode(string name, Animator animator, MultiAimConstraint headAim, bool canLookAtThePlayer, Transform target, DialogueTrigger dialogueTrigger)
    {
        if (dialogueIsPlaying)
            return;

        int index = levelDialogList.FindIndex(dialog => dialog.name == name);
        currentLevelDialog = levelDialogList[index];

        dialogueIsPlaying = true;
        //dialoguePanel = Instantiate(dialoguePanelPrefab, transform);
        dialoguePanel = Instantiate(dialoguePanelPrefab, transform.parent);

        this.target = target;
        dialoguePanel.transform.position = this.target.position;

        //dialogueText = dialoguePanel.transform.Find("text").GetComponent<TextMeshProUGUI>();
        //displayNameText = dialoguePanel.transform.Find("name").GetComponent<TextMeshProUGUI>();

        dialogueText = Helper.FindInChildren(dialoguePanel, "text").GetComponent<TextMeshProUGUI>();
        displayNameText = Helper.FindInChildren(dialoguePanel, "name").GetComponent<TextMeshProUGUI>();

        anim = animator;
        this.headAim = headAim;
        this.canLookAtThePlayer = canLookAtThePlayer;

        currentDialogueTrigger = dialogueTrigger;

        if (canLookAtThePlayer)
            headAim.weight = 1f;
        // reset portrait, layout, and speaker
        displayNameText.text = "???";

        currentLevelDialog.StartAgain();

        ContinueStory();
    }

    public IEnumerator ExitDialogueModeCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;

        onDialogEnd?.Invoke();

        Destroy(dialoguePanel);

        if (canLookAtThePlayer)
            headAim.weight = 0f;

        currentDialogueTrigger = null;

        SetCurrentAudioInfo(defaultAudioInfo.id);
    }

    void LoadLevelData()
    {
        // Load the TextAsset (without extension!)
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues"); // No .txt here
        if (jsonFile == null)
        {
            Debug.LogError("Could not load Notifications.json.txt from Resources.");
            return;
        }

        // Wrap the JSON string in a top-level object if needed
        string wrappedJson = "{ \"dialogues\": " + jsonFile.text + " }";

        // Deserialize into the LevelDataList
        DialogueList dataList = JsonUtility.FromJson<DialogueList>(wrappedJson);

        levelDialogList = dataList.dialogues;
    }

    public LevelDialog GetById(string dialogueId)
    {
        return levelDialogList.Find(dialogue => dialogue.name == dialogueId);
    }
}

[Serializable]
public class LevelDialog
{
    public string name;

    public List<Dialogue> dialogs;

    [TextArea(2, 5)]
    public string levelMissionText;

    [HideInInspector]
    public Dialogue currentDialog => dialogs[currentLineIndex];

    [HideInInspector]
    public bool canContinue = true;
    [HideInInspector]
    public int currentLineIndex = -1;

    public string Continue()
    {
        if (currentLineIndex >= dialogs.Count - 1)
        {
            canContinue = false;
            return "";
        }
        /*else if (currentLineIndex == dialogs.Count - 1)
        {
            canContinue = false;
            return dialogs[currentLineIndex].text;
        }*/
        else
        {
            currentLineIndex++;
            return dialogs[currentLineIndex].text;
        }
    }

    public void StartAgain()
    {
        currentLineIndex = -1;
        canContinue = true;
    }

}

[Serializable]
public class Dialogue
{
    [TextArea(3, 10)]
    public string text;

    public List<Choice> currentChoices;

    public List<string> currentTags;

    public int animationIndex;
    public int faceIndex;
}

[Serializable]
public class Choice
{
    [TextArea(2, 10)]
    public string text;
}

[Serializable]
public class DialogueList
{
    public List<LevelDialog> dialogues;
}
