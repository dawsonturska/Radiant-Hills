using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TMPDayDisplay : MonoBehaviour
{
    public TextMeshProUGUI dayText; // Assign in Inspector

    private void OnEnable()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Also check if the scene loaded is not Cutscene 2 right away when this object is enabled
        CheckSceneAndToggleVisibility();
    }

    private void OnDisable()
    {
        // Unsubscribe from scene load events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Perform visibility check on every scene load
        CheckSceneAndToggleVisibility();
    }

    private void CheckSceneAndToggleVisibility()
    {
        // If we are in Cutscene 2, disable the TMPDayDisplay
        if (SceneManager.GetActiveScene().name == "Cutscene 2")
        {
            dayText.gameObject.SetActive(false);
        }
        else
        {
            dayText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        // Proceed if DayCycleManager is present and the dayText is assigned
        if (DayCycleManager.Instance != null && dayText != null)
        {
            int day = DayCycleManager.Instance.currentDay;
            int timeOfDay = DayCycleManager.Instance.timeOfDay;

            string time = timeOfDay switch
            {
                0 => "Morning",
                1 => "Afternoon",
                2 => "Night",
                _ => "Unknown"
            };

            dayText.text = $"Day {day}, {time}";

            // Gradient colors based on time of day
            Color topColor;
            Color bottomColor;

            switch (timeOfDay)
            {
                case 0: // Morning
                    topColor = HexToColor("FFE5A0");
                    bottomColor = HexToColor("FFD580");
                    break;
                case 1: // Afternoon
                    topColor = HexToColor("A0D8FF"); // soft sky blue top
                    bottomColor = HexToColor("70C0FF"); // deeper blue base
                    break;
                case 2: // Night
                    topColor = HexToColor("A0AAFF");
                    bottomColor = HexToColor("8088FF");
                    break;
                default:
                    topColor = bottomColor = Color.white;
                    break;
            }

            VertexGradient gradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);
            dayText.colorGradient = gradient;
        }
    }

    // Helper to convert hex to Color
    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString($"#{hex}", out Color color))
        {
            return color;
        }
        Debug.LogWarning("Invalid hex color: " + hex);
        return Color.white;
    }
}
