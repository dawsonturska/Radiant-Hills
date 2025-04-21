using UnityEngine;
using UnityEngine.Rendering.Universal;  // Required for Light2D

public class DayNightLightController : MonoBehaviour
{
    [Tooltip("2D directional light to control.")]
    public Light2D directionalLight;

    [Tooltip("Time of day. 0 = Day, 1 = Night")]
    [Range(0, 1)]
    public int timeOfDay = 0;

    // Define colors using hex
    private readonly Color dayColor = new Color32(0xDC, 0xFF, 0xFE, 0xFF); // #DCFFFE
    private readonly Color nightColor = new Color32(0x28, 0x1B, 0x55, 0xFF); // #281B55

    private void Awake()
    {
        // Apply lighting based on initial time of day
        ApplyLighting();
    }

    private void OnValidate()
    {
        ApplyLighting(); // Update in editor when value changes
    }

    // Call this method whenever the time of day changes
    public void UpdateLighting(int newTimeOfDay)
    {
        timeOfDay = newTimeOfDay;
        ApplyLighting();
    }

    private void ApplyLighting()
    {
        if (directionalLight == null)
        {
            Debug.LogWarning("Directional light not assigned.");
            return;
        }

        switch (timeOfDay)
        {
            case 0: // Day
                directionalLight.intensity = 0.5f;
                directionalLight.color = dayColor;
                break;

            case 1: // Night
                directionalLight.intensity = 0.45f;
                directionalLight.color = nightColor;
                break;

            default:
                Debug.LogWarning("Unhandled timeOfDay value: " + timeOfDay);
                break;
        }
    }
}
