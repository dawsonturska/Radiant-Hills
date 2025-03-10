using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    public GameObject healthBar;     // The health bar GameObject (e.g., a Slider)
    public Slider healthSlider;      // The Slider component for the health bar
    public BossHealth bossHealth;    // Reference to the BossHealth script

    void Start()
    {
        // Validate that all references are assigned in the Inspector
        if (healthBar == null || healthSlider == null || bossHealth == null)
        {
            Debug.LogError("Missing references! Please assign the health bar, slider, and boss health in the inspector.");
            return;
        }

        // Initially hide the health bar
        healthBar.SetActive(false);

        // Subscribe to the OnHealthChanged event
        bossHealth.OnHealthChanged.AddListener(UpdateHealthBar);

        // Subscribe to the OnBossDied event
        bossHealth.OnBossDied.AddListener(HideHealthBar);

        // Ensure the slider's max value is set to the boss's max health
        healthSlider.maxValue = bossHealth.maxHealth;
        healthSlider.value = bossHealth.GetCurrentHealth();  // Initial value
    }

    // Show or hide the health bar based on the aggro state
    public void ToggleHealthBar(bool show)
    {
        healthBar.SetActive(show);
    }

    // Hide the health bar when the boss dies
    private void HideHealthBar()
    {
        healthBar.SetActive(false);  // Hide the health bar
    }

    // Unsubscribe from events to avoid memory leaks
    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
            bossHealth.OnBossDied.RemoveListener(HideHealthBar);
        }
    }

    // Update the health bar when health changes
    public void UpdateHealthBar(float currentHealth)  // This method now accepts a float parameter
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;  // Update slider value to the current health
        }
    }
}
