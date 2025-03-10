using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    public float lifetime = 5f;
    public float damage = 10f;
    public float moveSpeed = 5f;

    [Header("References")]
    public TurretLogic turretLogic;

    private Rigidbody2D rb;
    private Vector3 moveDirection;
    private bool isLaunched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}: Rigidbody2D component is missing!");
        }
    }

    void Start()
    {
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    public void Initialize(TurretLogic turret, Vector3 direction)
    {
        turretLogic = turret;
        moveDirection = direction.normalized;

        if (rb != null)
        {
            rb.velocity = moveDirection * moveSpeed;
            isLaunched = true;
        }
    }

    void Update()
    {
        if (isLaunched && rb != null && rb.velocity.magnitude < moveSpeed)
        {
            rb.velocity = moveDirection * moveSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Check if we collided with a ProjectileLock object
        ProjectileLock lockObject = collision.gameObject.GetComponent<ProjectileLock>();
        if (lockObject != null)
        {
            lockObject.Unlock(); // Unlock the lock object so the player can walk through
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (turretLogic != null)
        {
            turretLogic.OnProjectileDestroyed(gameObject);
        }

        Destroy(gameObject);
    }
}
