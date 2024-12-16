using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent OnHealthChanged;  // Event for health changes
    public UnityEvent OnBossDied;       // Event for when the boss dies

    void Start()
    {
        currentHealth = maxHealth;

        // If OnHealthChanged or OnBossDied are not initialized, create new UnityEvents
        if (OnHealthChanged == null)
            OnHealthChanged = new UnityEvent();
        if (OnBossDied == null)
            OnBossDied = new UnityEvent();
    }

    // Method to apply damage to the boss
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            // Trigger the health change event
            OnHealthChanged.Invoke();  // Notify listeners about the health change
        }
    }

    // Method to handle boss death
    private void Die()
    {
        Debug.Log("Boss died!");
        OnBossDied.Invoke();  // Notify listeners that the boss is dead
        Destroy(gameObject);  // Destroy the boss object (you can replace it with your own death logic)
    }

    // Optionally, a method to get current health
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
