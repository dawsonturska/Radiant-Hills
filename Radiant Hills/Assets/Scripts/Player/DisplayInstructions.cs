using UnityEngine;
using System.Collections;

public class DisplayInstructions : MonoBehaviour
{
    private string currentMessage = ""; // Message to be displayed
    private bool isDisplaying = false; // Flag to track if a message is being displayed

    void Start()
    {
        // Start the coroutine to display messages
        StartCoroutine(DisplayMessageSequence());
    }

    // Coroutine to display the messages with delays
    IEnumerator DisplayMessageSequence()
    {
        // First message
        currentMessage = "Use WASD to move.";
        Debug.Log(currentMessage);
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Second message
        currentMessage = "Press Space to Attack.";
        Debug.Log(currentMessage);
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Third message
        currentMessage = "Press E to Interact.";
        Debug.Log(currentMessage);
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Clear the current message after all are displayed
        currentMessage = "";
        Debug.Log(currentMessage);
    }
}
