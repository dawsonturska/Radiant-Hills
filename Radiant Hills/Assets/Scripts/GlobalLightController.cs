using UnityEngine;
using UnityEngine.Rendering.Universal; // For Light2D

public class GlobalLightController : MonoBehaviour
{
    [Tooltip("2D Global Light to control.")]
    public Light2D globalLight;

    [Tooltip("Time of day. 0 = Day, 1 = Night")]
    [Range(0, 1)]
    public int timeOfDay = 0;

    private void Awake()
    {
        ApplyLighting();
    }

    private void OnValidate()
    {
        ApplyLighting(); // Live updates in Inspector
    }

    // Call this from DayCycleManager or other controller
    public void UpdateLighting(int newTimeOfDay)
    {
        timeOfDay = newTimeOfDay;
        ApplyLighting();
    }

    private void ApplyLighting()
    {
        if (globalLight == null)
        {
            Debug.LogWarning("Global Light not assigned.");
            return;
        }

        switch (timeOfDay)
        {
            case 0: // Day
                globalLight.intensity = 1f;
                break;

            case 1: // Night
                globalLight.intensity = 0.05f;
                break;

            default:
                Debug.LogWarning("Unhandled timeOfDay value: " + timeOfDay);
                break;
        }
    }
}
