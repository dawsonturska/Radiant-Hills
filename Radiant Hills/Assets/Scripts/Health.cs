using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 20; // Starting health of the game object

    private bool isDead = false; // Flag to check if the player is dead

    // Method to take damage
    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prevent damage if already dead

        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, health is now {health}");

        // Check if health has dropped below 0
        if (health <= 0)
        {
            Die();
        }
    }

    // Method called when health reaches 0 or below
    private void Die()
    {
        if (isDead) return; // Prevent multiple deaths

        isDead = true; // Mark as dead
        Debug.Log($"{gameObject.name} has died.");

        // Destroy the game object or trigger a death animation/logic
        Destroy(gameObject); // Or replace with death animation if desired
    }
}