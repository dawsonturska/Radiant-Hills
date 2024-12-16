using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;  // Max health of the player
    public float currentHealth;     // Current health of the player
    public Slider healthBar;        // Reference to the health bar UI slider

    void Start()
    {
        // Initialize the player's health to max health
        currentHealth = maxHealth;

        // Ensure the health bar is initialized
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;  // Set the health bar's value to current health
        }
    }

    // Method to apply damage to the player
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Update the health bar
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Check if the player is dead (optional)
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method for when the player dies (optional)
    void Die()
    {
        Debug.Log("Player has died!");
        // You can add death logic here, like triggering an animation or restarting the game
    }
}
