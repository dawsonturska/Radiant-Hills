using TMPro;
using UnityEngine;

public class TMPDayDisplay : MonoBehaviour
{
    public TextMeshProUGUI dayText; // Assign in Inspector

    private void Update()
    {
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
