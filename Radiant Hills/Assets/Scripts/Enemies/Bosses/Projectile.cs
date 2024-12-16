using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f; // Time until the projectile is destroyed
    public float damage = 10f; // Damage dealt by the projectile
    public float moveSpeed = 5f; // Speed of the projectile
    public float hoverDuration = 1f; // Time before moving towards the player
    public Transform boss; // Reference to the boss (could also be set via Inspector)
    public CentipedeBehavior centipedeBehavior; // Reference to CentipedeBehavior

    private Transform player;
    private bool isLaunched = false; // If the projectile is moving towards the player
    private bool isReflected = false; // If the projectile is reflected back to the boss
    private float hoverTimer = 0f; // Timer for the hover phase
    private Rigidbody2D rb;
    private Collider2D projectileCollider;

    void Start()
    {
        if (boss == null)
        {
            // Attempt to get the CentipedeBehavior if not manually assigned
            boss = transform.root; // Assuming the boss is the parent of the projectile
            centipedeBehavior = boss.GetComponent<CentipedeBehavior>();

            if (centipedeBehavior == null)
            {
                Debug.LogError("CentipedeBehavior not found on the boss.");
                return;
            }
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        // Ignore collision with the boss collider immediately
        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                IgnoreBossCollider(bossCollider, true);
            }
        }

        // Schedule destruction after the projectile's lifetime
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    void Update()
    {
        if (!isLaunched)
        {
            // Hover phase before launching towards the player
            hoverTimer += Time.deltaTime;

            if (hoverTimer >= hoverDuration)
            {
                isLaunched = true;
            }
        }
        else if (!isReflected)
        {
            // Move towards the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            rb.velocity = directionToPlayer * moveSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon") && !isReflected)
        {
            ReflectProjectileBackToBoss();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            if (isReflected)
            {
                // Logic for hitting the boss after reflection
                Debug.Log("Reflected projectile hit the boss!");

                // Apply damage to the boss after reflection
                CentipedeBehavior centipedeBehavior = collision.gameObject.GetComponent<CentipedeBehavior>();
                if (centipedeBehavior != null)
                {
                    // Apply the damage to the boss
                    centipedeBehavior.TakeDamage((int)damage); // Assuming the damage is castable to int
                }

                // Notify the CentipedeBehavior to teleport
                centipedeBehavior.TeleportToRandomLocation();

                DestroyProjectile();
            }
            else
            {
                // If the projectile hasn't been reflected yet, destroy it without damaging the boss
                DestroyProjectile();
            }
        }
        else
        {
            DestroyProjectile();
        }
    }

    private void ReflectProjectileBackToBoss()
    {
        isReflected = true;

        // Change direction towards the boss
        Vector3 directionToBoss = (boss.position - transform.position).normalized;
        if (rb != null)
        {
            rb.velocity = directionToBoss * moveSpeed * 2f; // Reflected speed is doubled
        }

        // Allow collision with the boss during reflection
        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                IgnoreBossCollider(bossCollider, false);
            }
        }

        Debug.Log("Projectile reflected back to the boss!");
    }

    public void IgnoreBossCollider(Collider2D bossCollider, bool ignore)
    {
        if (projectileCollider != null && bossCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, bossCollider, ignore);
        }
    }

    private void DestroyProjectile()
    {
        // Notify the boss that the projectile was destroyed
        if (centipedeBehavior != null)
        {
            centipedeBehavior.OnProjectileDestroyed(gameObject);
        }

        Destroy(gameObject);
    }
}
