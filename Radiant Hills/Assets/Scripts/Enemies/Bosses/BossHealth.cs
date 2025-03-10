using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    // Event for health changes with current health info
    public UnityEvent<float> OnHealthChanged;

    // Event for when the boss dies
    public UnityEvent OnBossDied;

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize UnityEvents if not already initialized (good practice)
        if (OnHealthChanged == null)
            OnHealthChanged = new UnityEvent<float>();
        if (OnBossDied == null)
            OnBossDied = new UnityEvent();

        // Optionally invoke OnHealthChanged once at start to update listeners immediately
        OnHealthChanged.Invoke(currentHealth);
    }

    // Method to apply damage to the boss
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Ensure current health doesn't go below zero
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            // Trigger the health change event with the current health
            OnHealthChanged.Invoke(currentHealth);  // Pass the current health to listeners
        }
    }

    // Method to heal the boss (optional)
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Heal without exceeding max health
        OnHealthChanged.Invoke(currentHealth);  // Trigger health change event
    }

    // Method to handle boss death
    private void Die()
    {
        Debug.Log("Boss died!");
        OnBossDied.Invoke();  // Notify listeners that the boss is dead
        Destroy(gameObject);  // Destroy the boss object (replace with your own death logic if needed)
    }

    // Optionally, a method to get current health
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
