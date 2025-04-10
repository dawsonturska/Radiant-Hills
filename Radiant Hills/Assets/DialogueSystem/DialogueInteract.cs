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
    //[SerializeField] private AudioSource charaAudioSource;
    [SerializeField] private List<GameObject> choiceButtons;
    //[SerializeField] private UnityEvent[] eventQueue;

    // New checkbox option to automatically start dialogue on scene load
    [SerializeField] private bool startDialogueOnSceneLoad = false; // Enable this in the Inspector

    bool optionSelected = false;

    private void Start()
    {
        // Automatically start dialogue if the checkbox is checked
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

    IEnumerator DisplayDialogue(DialogueObject _dialogueObject)
    {
        yield return null;
        dialogueCanvas.enabled = true;
        foreach (var dialogue in _dialogueObject.dialogueSegments)
        {
            speakerText.text = dialogue.speakerText;
            charSprite.sprite = dialogue.charSprite;

            //if (charaAudioSource != null && dialogue.charAudio != null)
            //{
            //charaAudioSource.Stop();
            //charaAudioSource.clip = dialogue.charAudio;
            //charaAudioSource.Play();
            //}

            if (speakerText.text != "")
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
                while (!Input.GetKeyDown(KeyCode.Space) || !Input.GetMouseButtonDown(0))
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) break;
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

                while (!optionSelected)
                {
                    yield return null;
                }
            }

            //if (eventQueue != null && dialogue.useQueuedEvent)
            //{
            //if (dialogue.queuedEvent > -1 || dialogue.queuedEvent <= eventQueue.Length) eventQueue[dialogue.queuedEvent].Invoke();
            //}
        }
        dialogueOptionsContainer.SetActive(false);
        dialogueCanvas.enabled = false;
        optionSelected = false;

        foreach (var button in choiceButtons)
        {
            if (button.gameObject.activeSelf == true) button.gameObject.SetActive(false);
        }
    }
}
