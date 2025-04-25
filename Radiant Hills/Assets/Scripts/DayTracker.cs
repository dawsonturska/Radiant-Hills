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
            bool isMorning = DayCycleManager.Instance.timeOfDay == 0;
            string time = isMorning ? "Morning" : "Night";
            dayText.text = $"Day {day}, {time}";

            // Update color1 of vertical gradient (bottomLeft for TMP)
            VertexGradient gradient = dayText.colorGradient;
            gradient.bottomLeft = isMorning ? HexToColor("FFE5A0") : HexToColor("A0AAFF");
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
