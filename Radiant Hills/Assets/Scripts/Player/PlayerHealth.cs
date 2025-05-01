using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;  // Needed for UnityEvent

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;          // Max health of the player
    public float currentHealth;             // Current health of the player
    public Slider healthBar;                // Reference to the health bar UI slider

    [Header("Events")]
    public UnityEvent OnPlayerDied;         // Event to fire when the player dies

    void Start()
    {
        // Initialize the player's health to max health
        currentHealth = maxHealth;

        // Ensure the health bar is initialized
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // Method to apply damage to the player
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method for when the player dies
    void Die()
    {
        Debug.Log("Player has died!");

        // Fire death event
        OnPlayerDied?.Invoke();
    }
}
