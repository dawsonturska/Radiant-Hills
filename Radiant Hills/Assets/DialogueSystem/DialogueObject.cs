using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueObject", menuName = "Project - CodeLibrary/DialogueObject")]
public class DialogueObject : ScriptableObject {

    [Header("Dialogue")]
    public List<DialogueSegment> dialogueSegments = new List<DialogueSegment>();


}

[System.Serializable]
public struct DialogueSegment { 
    public string dialogueText;
    public string speakerText;
    public Sprite charSprite;
    public Sprite phoneSprite;
    public int queuedEvent;
    public bool useQueuedEvent;
    public AudioClip charAudio;
    public List<DialogueChoice> dialogueChoices; 
    }

[System.Serializable]
public struct DialogueChoice{
    public string dialogueChoice;
    public DialogueObject followOnDialogue;
}