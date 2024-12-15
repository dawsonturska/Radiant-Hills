using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int health = 20; // Starting health of the object
    public MaterialType materialType; // Material type of the object (e.g., Rock, Wood)
    public int materialYield = 1;    // Number of materials yielded when broken
    public GameObject breakEffect;  // Optional: Particle effect when the object breaks
    public AudioClip breakSound;    // Optional: Sound effect when the object breaks

    private bool isBroken = false;  // Flag to prevent multiple breaks
    private AudioSource audioSource; // Optional: AudioSource for break sound

    private void Start()
    {
        // Optional: Attach an AudioSource if a break sound is provided
        if (breakSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = breakSound;
            audioSource.playOnAwake = false;
        }
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        if (isBroken) return; // Prevent breaking if already broken

        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, health is now {health}");

        // Check if health has dropped to 0 or below
        if (health <= 0)
        {
            BreakObject();
        }
    }

    // Method to break the object
    private void BreakObject()
    {
        if (isBroken) return; // Prevent multiple break events

        isBroken = true; // Mark the object as broken

        // Optional: Play break sound
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Optional: Instantiate break effect (e.g., particles)
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        // Add materials to player's inventory
        GameObject player = GameObject.FindWithTag("Player");
        Inventory inventory = player?.GetComponent<Inventory>();

        if (inventory != null && materialType != null)
        {
            // Pass the quantity (materialYield) to the AddMaterial method
            inventory.AddMaterial(materialType, materialYield);
            Debug.Log($"Collected {materialYield} {materialType.materialName}(s)");
        }
        else
        {
            Debug.LogError("Inventory or MaterialType is missing!");
        }

        // Destroy the object after breaking
        Destroy(gameObject, 0.1f); // Optional: Slight delay for effect and sound
    }
}
