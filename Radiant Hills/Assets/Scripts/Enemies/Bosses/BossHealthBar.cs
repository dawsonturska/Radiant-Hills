using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    public GameObject healthBar;         // The health bar GameObject (e.g., a Slider)
    public Slider healthSlider;          // The Slider component for the health bar
    public BossHealth bossHealth;        // Reference to the BossHealth script

    [Header("Optional UI Elements")]
    public SpriteRenderer extraImage;             // An extra graphic/image for the health bar (e.g., frame or glow)
    public bool showExtraImage = true;   // Toggle to show or hide the extra image

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

        // Set the visibility of the extra image
        if (extraImage != null)
        {
            extraImage.enabled = showExtraImage;
        }

        // Subscribe to the OnHealthChanged event
        bossHealth.OnHealthChanged.AddListener(UpdateHealthBar);

        // Subscribe to the OnBossDied event
        bossHealth.OnBossDied.AddListener(HideHealthBar);

        // Ensure the slider's max value is set to the boss's max health
        healthSlider.maxValue = bossHealth.maxHealth;
        healthSlider.value = bossHealth.GetCurrentHealth();  // Initial value
    }

    public void ToggleHealthBar(bool show)
    {
        healthBar.SetActive(show);

        // Optionally also toggle the extra image
        if (extraImage != null)
        {
            extraImage.enabled = show && showExtraImage;
        }
    }

    private void HideHealthBar()
    {
        healthBar.SetActive(false);

        if (extraImage != null)
        {
            extraImage.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
            bossHealth.OnBossDied.RemoveListener(HideHealthBar);
        }
    }

    public void UpdateHealthBar(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}
