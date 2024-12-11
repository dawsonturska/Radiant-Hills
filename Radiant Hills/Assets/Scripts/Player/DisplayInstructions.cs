using UnityEngine;
using System.Collections;

public class DisplayInstructions : MonoBehaviour
{
    private GUIStyle guiStyle;
    private string currentMessage = ""; // Message to be displayed
    private bool isDisplaying = false; // Flag to track if a message is being displayed

    void Start()
    {
        // Set up the GUI style for displaying the messages
        guiStyle = new GUIStyle
        {
            fontSize = 20,
            normal = new GUIStyleState { textColor = Color.white },
            wordWrap = true
        };

        // Start the coroutine to display messages
        StartCoroutine(DisplayMessageSequence());
    }

    // Coroutine to display the messages with delays
    IEnumerator DisplayMessageSequence()
    {
        // First message
        currentMessage = "Use WASD to move.";
        yield return new WaitForSeconds(3f); // Wait for 1 second

        // Second message
        currentMessage = "Press Space to Attack.";
        yield return new WaitForSeconds(3f); // Wait for 1 second

        // Third message
        currentMessage = "Press E to Interact.";
        yield return new WaitForSeconds(3f); // Wait for 1 second

        // After all messages, stop displaying further messages
        currentMessage = "";
    }

    // OnGUI method to display the current message
    void OnGUI()
    {
        if (!string.IsNullOrEmpty(currentMessage))
        {
            // Display the current message on the screen
            GUI.Label(new Rect(10, 20, Screen.width - 20, 100), currentMessage, guiStyle);
        }
    }
}