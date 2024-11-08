using UnityEngine;

public class Combatant : MonoBehaviour
{
    public float health = 100f;
    public float attackDamage = 10f;
    public float attackInterval = 2f;
    private float attackTimer;

    public Combatant opponent;
    public EnemyManager enemyManager; // Reference to the enemy manager

    void Start()
    {
        attackTimer = attackInterval;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    void Attack()
    {
        if (opponent != null)
        {
            opponent.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{name} takes {damage} damage. Health left: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} has died.");

        // Notify the enemy manager if this is an enemy
        if (enemyManager != null)
        {
            enemyManager.EnemyDied();
        }

        Destroy(gameObject);
    }
}