using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayCycleManager : MonoBehaviour
{
    public int currentDay = 1;        // Starts at day 1
    public int timeOfDay = 0;         // 0 = morning, 1 = afternoon, 2 = night

    [Header("Lights to Control")]
    public Light2D[] lightsToControl;   // List of Light2D objects to update

    public static DayCycleManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            SceneManager.sceneLoaded += OnSceneLoaded;      // Add this line
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadDay(); // Load saved day/time when game first starts
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadDay(); // Reload lighting after a new scene loads
    }

    public void AdvanceTime()
    {
        timeOfDay++;

        if (timeOfDay > 2)
        {
            timeOfDay = 0;
            currentDay++;
        }

        UpdateLighting();
        SaveDay();

        string timeLabel = timeOfDay switch
        {
            0 => "Morning",
            1 => "Afternoon",
            2 => "Night",
            _ => "Unknown"
        };

        Debug.Log($"Day {currentDay}, Time of Day: {timeLabel}");
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
            if (light == null) continue;

            switch (timeOfDay)
            {
                case 0: // Morning
                    light.intensity = 0.5f;
                    light.color = new Color(0xDC / 255f, 0xFF / 255f, 0xFE / 255f); // #DCFFFE
                    break;

                case 1: // Afternoon
                    light.intensity = 0.8f;
                    light.color = new Color(1f, 0.98f, 0.8f); // warm afternoon light
                    break;

                case 2: // Night
                    light.intensity = 0.25f;
                    light.color = new Color(0x28 / 255f, 0x1B / 255f, 0x55 / 255f); // #281B55
                    break;

                default:
                    Debug.LogWarning("Unhandled timeOfDay value: " + timeOfDay);
                    break;
            }
        }
    }

    public void SetTimeOfDay(int newTimeOfDay)
    {
        if (newTimeOfDay >= 0 && newTimeOfDay <= 2 && newTimeOfDay != timeOfDay)
        {
            timeOfDay = newTimeOfDay;
            UpdateLighting();
        }
    }

    public void SaveDay()
    {
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        PlayerPrefs.SetInt("TimeOfDay", timeOfDay);
        PlayerPrefs.Save();
    }

    public void LoadDay()
    {
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        timeOfDay = PlayerPrefs.GetInt("TimeOfDay", 0);
        UpdateLighting();
    }
}
