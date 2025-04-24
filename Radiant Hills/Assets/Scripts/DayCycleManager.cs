using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycleManager : MonoBehaviour
{
    public int currentDay = 1;        // Starts at day 1
    public int timeOfDay = 0;         // 0 = morning, 1 = night

    [Header("Lights to Control")]
    public Light2D[] lightsToControl;   // List of Light2D objects to update

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
            return;
        }
    }

    void Start()
    {
        // Apply correct lighting when the scene starts
        UpdateLighting();
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

        UpdateLighting();

        Debug.Log($"Day {currentDay}, Time of Day: {(timeOfDay == 0 ? "Morning" : "Night")}");
    }

    public void UpdateLighting()
    {
        if (lightsToControl == null || lightsToControl.Length == 0)
        {
            Debug.LogWarning("No lights assigned to control.");
            return;
        }

        foreach (Light2D light in lightsToControl)
        {
            if (light != null)
            {
                switch (timeOfDay)
                {
                    case 0: // Morning
                        light.intensity = 0.5f;
                        light.color = new Color(0xDC / 255f, 0xFF / 255f, 0xFE / 255f); // #DCFFFE
                        break;

                    case 1: // Night
                        light.intensity = 0.25f;
                        light.color = new Color(0x28 / 255f, 0x1B / 255f, 0x55 / 255f); // #281B55
                        break;

                    default:
                        Debug.LogWarning("Unhandled timeOfDay value: " + timeOfDay);
                        break;
                }
            }
        }
    }

    public void SetTimeOfDay(int newTimeOfDay)
    {
        if (newTimeOfDay != timeOfDay)
        {
            timeOfDay = newTimeOfDay;
            UpdateLighting();
        }
    }

    public void SaveDay()
    {
        PlayerPrefs.SetInt("CurrentDay", currentDay);      // Save current day
        PlayerPrefs.SetInt("TimeOfDay", timeOfDay);        // Save time of day (morning or night)
        PlayerPrefs.Save();
    }

    public void LoadDay()
    {
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1);  // Default to day 1 if not saved
        timeOfDay = PlayerPrefs.GetInt("TimeOfDay", 0);    // Default to morning if not saved
        UpdateLighting(); // Ensure the correct lighting based on time of day
    }
}
