using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDialogueOption : MonoBehaviour
{
    DialogueInteract dialogueInteract;
    DialogueObject dialogueObject;

    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] GameObject charSprite;


    public void Setup (DialogueInteract _dialogueInteract, DialogueObject _dialogueObject, string _dialogueText) {
        dialogueInteract = _dialogueInteract;
        dialogueObject = _dialogueObject;
        dialogueText.text = _dialogueText;

    }

    public void SelectOption () {
        dialogueInteract.OptionSelected(dialogueObject);
    }
}
