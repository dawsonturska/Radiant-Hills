using UnityEngine;

public class HealthBarVisibility : MonoBehaviour
{
    public GameObject healthBar;  // Reference to the health bar GameObject

    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar reference not set!");
            return;
        }

        // Initially hide the health bar in the editor
#if UNITY_EDITOR
        healthBar.SetActive(false);  // Make health bar invisible in the editor
#endif
    }

    void Update()
    {
        // Toggle the health bar visibility when the game starts
        if (Application.isPlaying)
        {
            healthBar.SetActive(true);  // Make health bar visible during gameplay
        }
    }
}
