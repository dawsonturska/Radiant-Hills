using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    public int currentDay = 1;        // Starts at day 1
    public int timeOfDay = 0;         // 0 = morning, 1 = night

    // Optional: Singleton if you want global access
    public static DayCycleManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this to move from morning to night, or night to next morning
    public void AdvanceTime()
    {
        if (timeOfDay == 0)
        {
            timeOfDay = 1; // Morning -> Night
        }
        else
        {
            timeOfDay = 0; // Night -> Morning
            currentDay++;  // Increment the day
        }

        Debug.Log($"Day {currentDay}, Time of Day: {(timeOfDay == 0 ? "Morning" : "Night")}");
    }
}
