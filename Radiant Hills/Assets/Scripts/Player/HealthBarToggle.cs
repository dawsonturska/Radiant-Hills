using UnityEngine;
using UnityEngine.SceneManagement; // Needed to check the scene name

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
        // Check if the current scene is "Gather"
        if (SceneManager.GetActiveScene().name == "Gather")
        {
            // Toggle the health bar visibility during the Gather scene
            if (Application.isPlaying)
            {
                healthBar.SetActive(true);  // Make health bar visible during gameplay
            }
        }
        else
        {
            // Hide the health bar in all other scenes
            healthBar.SetActive(false);
        }
    }
}
