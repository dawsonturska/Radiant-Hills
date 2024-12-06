using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    public int damage = 10; // Damage to deal on collision

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure this collision is only counted once
        // Check if the object has a Health component
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            // Deal damage to the object
            targetHealth.TakeDamage(damage);
            Debug.Log($"{other.gameObject.name} took {damage} damage.");

            // Disable the collider to prevent further collisions
            GetComponent<Collider2D>().enabled = false;

            // Destroy the attack prefab after 0.5 seconds
            Destroy(gameObject, 0.5f);
        }
    }
}