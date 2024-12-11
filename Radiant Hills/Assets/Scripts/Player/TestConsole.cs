using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsoleToScreen : MonoBehaviour
{
    private class LogMessage
    {
        public string message;
        public float timeReceived;

        public LogMessage(string message, float timeReceived)
        {
            this.message = message;
            this.timeReceived = timeReceived;
        }
    }

    private List<LogMessage> logMessages = new List<LogMessage>(); // Store log messages with their timestamp
    private int maxMessages = 10; // Maximum number of messages to display
    private float messageDecayTime = 3f; // Time in seconds after which the message decays
    private GUIStyle guiStyle;

    // This method captures Unity's log messages
    void OnEnable()
    {
        Application.logMessageReceived += ConsoleLogMessage;
        guiStyle = new GUIStyle
        {
            fontSize = 20,
            normal = new GUIStyleState { textColor = Color.white },
            wordWrap = true
        };

        // Start the coroutine to check expired messages every 1 second
        StartCoroutine(CheckMessageDecay());
    }

    // This method is called when a log message is received
    void ConsoleLogMessage(string logString, string stackTrace, LogType type)
    {
        // Add the new log message to the list with the current time
        logMessages.Add(new LogMessage(logString, Time.time));

        // Limit the number of messages to display
        if (logMessages.Count > maxMessages)
        {
            logMessages.RemoveAt(0); // Remove the oldest message if we've exceeded maxMessages
        }
    }

    // This coroutine runs every 1 second to check and remove expired messages
    IEnumerator CheckMessageDecay()
    {
        while (true)
        {
            // Remove expired messages
            logMessages.RemoveAll(msg => Time.time - msg.timeReceived > messageDecayTime);
            yield return new WaitForSeconds(1f); // Wait for 1 second before checking again
        }
    }

    // This method displays the messages on the screen
    void OnGUI()
    {
        float yOffset = 20;

        // Display each log message
        foreach (var log in logMessages)
        {
            GUI.Label(new Rect(10, yOffset, Screen.width - 20, 100), log.message, guiStyle);
            yOffset += 30; // Increase the offset for each new line
        }
    }

    // Don't forget to remove the listener when the object is destroyed to avoid memory leaks
    void OnDisable()
    {
        Application.logMessageReceived -= ConsoleLogMessage;
    }
}
