using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    public GameObject healthBar;     // The health bar GameObject (e.g., a Slider)
    public Slider healthSlider;      // The Slider component for the health bar
    public BossHealth bossHealth;    // Reference to the BossHealth script

    void Start()
    {
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
    }

    void Update()
    {
        // Continuously update the health bar
        if (bossHealth != null)
        {
            healthSlider.value = bossHealth.GetCurrentHealth();  // Use raw health for slider
        }
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

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
            bossHealth.OnBossDied.RemoveListener(HideHealthBar);
        }
    }

    // Update the health bar when health changes
    public void UpdateHealthBar()
    {
        if (bossHealth != null && healthSlider != null)
        {
            healthSlider.value = bossHealth.GetCurrentHealth();  // Update to the current health
        }
    }
}
