using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public float timeRemaining = 300f; // Initial timer value set to 5 minutes (300 seconds)
    public Text timerText; // Reference to the UI Text element

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Decrease time by the time since the last frame
            UpdateTimerText(); // Update the UI Text
        }
        else
        {
            timeRemaining = 0; // Clamp to 0
            Debug.Log("Time's up!");
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(timeRemaining % 60); // Calculate seconds
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format time as MM:SS
    }
}
