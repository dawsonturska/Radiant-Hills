using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueInteract : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private TextMeshProUGUI dialogueNoSpeakerText;
    [SerializeField] private TextMeshProUGUI dialogueSpeakerText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private Image charSprite;
    [SerializeField] private GameObject dialogueOptionsContainer;
    [SerializeField] private Transform dialogueOptionsParent;
    [SerializeField] private GameObject dialogueOptionsButtonPrefab;
    [SerializeField] private DialogueObject startDialogueObject;
    [SerializeField] private Sprite noSpeakerSprite;
    [SerializeField] private Sprite speakerSprite;
    [SerializeField] private List<GameObject> choiceButtons;
    [SerializeField] private UnityEvent[] eventQueue;
    [SerializeField] private bool startDialogueOnSceneLoad = false;
    [SerializeField] private Button closeButton;

    private bool optionSelected = false;

    private void Awake()
    {
        HideDialogueUI();

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDialogue);
    }

    private void Start()
    {
        if (startDialogueOnSceneLoad)
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        StartCoroutine(DisplayDialogue(startDialogueObject));
    }

    public void StartDialogue(DialogueObject _dialogueObject)
    {
        StartCoroutine(DisplayDialogue(_dialogueObject));
    }

    public void OptionSelected(DialogueObject selectedOption)
    {
        optionSelected = true;
        StartDialogue(selectedOption);
    }

    public void CloseDialogue()
    {
        StopAllCoroutines();
        HideDialogueUI();
        optionSelected = false;
    }

    private void HideDialogueUI()
    {
        dialogueCanvas.enabled = false;
        dialogueSpeakerText.gameObject.SetActive(false);
        dialogueNoSpeakerText.gameObject.SetActive(false);
        dialogueBox.enabled = false;
        charSprite.gameObject.SetActive(false);
        dialogueOptionsContainer.SetActive(false);

        foreach (var button in choiceButtons)
        {
            button.SetActive(false);
        }

        if (closeButton != null)
            closeButton.gameObject.SetActive(false);
    }

    IEnumerator DisplayDialogue(DialogueObject _dialogueObject)
    {
        HideDialogueUI(); // double safety
        yield return null;

        dialogueCanvas.enabled = true;
        dialogueBox.enabled = true;

        if (closeButton != null)
            closeButton.gameObject.SetActive(true);

        foreach (var dialogue in _dialogueObject.dialogueSegments)
        {
            speakerText.text = dialogue.speakerText;
            charSprite.sprite = dialogue.charSprite;

            if (!string.IsNullOrEmpty(speakerText.text))
            {
                dialogueSpeakerText.gameObject.SetActive(true);
                dialogueNoSpeakerText.gameObject.SetActive(false);
                dialogueSpeakerText.text = dialogue.dialogueText;
                dialogueBox.sprite = speakerSprite;
                charSprite.gameObject.SetActive(true);
            }
            else
            {
                dialogueSpeakerText.gameObject.SetActive(false);
                dialogueNoSpeakerText.gameObject.SetActive(true);
                dialogueNoSpeakerText.text = dialogue.dialogueText;
                dialogueBox.sprite = noSpeakerSprite;
                charSprite.gameObject.SetActive(false);
            }

            if (dialogue.dialogueChoices.Count == 0)
            {
                while (true)
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                        break;
                    yield return null;
                }

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                dialogueOptionsContainer.SetActive(true);
                int i = 0;

                foreach (var option in dialogue.dialogueChoices)
                {
                    GameObject newButton = choiceButtons[i];
                    newButton.SetActive(true);
                    newButton.GetComponent<UIDialogueOption>().Setup(this, option.followOnDialogue, option.dialogueChoice);
                    i++;
                }

                optionSelected = false;
                while (!optionSelected)
                {
                    yield return null;
                }
            }

            if (eventQueue != null && dialogue.useQueuedEvent)
            {
                if (dialogue.queuedEvent >= 0 && dialogue.queuedEvent < eventQueue.Length)
                {
                    if (eventQueue[dialogue.queuedEvent] != null)
                    {
                        eventQueue[dialogue.queuedEvent].Invoke();
                    }
                    else
                    {
                        Debug.LogError("The event in the queue at index " + dialogue.queuedEvent + " is null.");
                    }
                }
                else
                {
                    Debug.LogError("Queued event index out of range: " + dialogue.queuedEvent);
                }
            }
        }

        HideDialogueUI();
        optionSelected = false;
    }
}
