using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    public int damage = 10;  // Damage to deal on collision
    public float knockbackForce = 10f;  // The force with which the enemy is knocked back
    public float maxKnockbackDistance = 3f;  // Maximum knockback distance (3 meters)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object has a BreakableObject component
        BreakableObject targetBreakable = other.GetComponent<BreakableObject>();
        if (targetBreakable != null)
        {
            targetBreakable.TakeDamage(damage);
            //Debug.Log($"{other.gameObject.name} took {damage} damage (BreakableObject).");
            DisableAndDestroyAttack();
        }

        // Check if the object has an Enemy component
        Enemy targetEnemy = other.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            // Deal damage to the Enemy
            targetEnemy.TakeDamage(damage);
            Debug.Log($"{other.gameObject.name} took {damage} damage (Enemy).");

            // Apply knockback to the enemy (only if the enemy has a Rigidbody2D)
            Rigidbody2D rb = targetEnemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calculate knockback direction: from enemy to the attack's origin
                Vector2 knockbackDirection = targetEnemy.transform.position - transform.position;
                knockbackDirection.Normalize();  // Normalize to just get the direction

                // Limit the knockback distance to a maximum value (Clamp the magnitude)
                knockbackDirection = Vector2.ClampMagnitude(knockbackDirection, maxKnockbackDistance);

                // Apply the knockback force in the calculated direction
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            DisableAndDestroyAttack();
        }
    }

    // Function to disable the collider and destroy the attack object after a short delay
    private void DisableAndDestroyAttack()
    {
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}