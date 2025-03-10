using UnityEngine;

public class ProjectileReflector : MonoBehaviour
{
    [Header("Targeting & Firing")]
    public Transform focalPoint; // The point from which the reflector fires projectiles
    public GameObject projectilePrefab; // The projectile prefab to fire
    public float fireRate = 1f; // Time between each reflected projectile shot
    public float spawnOffset = 1f; // Offset to prevent self-collision

    private float fireRateTimer = 0f; // Timer to manage the fire rate

    void Start()
    {
        // Validate required references
        if (focalPoint == null)
            Debug.LogError($"{gameObject.name}: Focal point is not assigned!");

        if (projectilePrefab == null)
            Debug.LogError($"{gameObject.name}: Projectile prefab is not assigned!");
    }

    void Update()
    {
        // Handle fire rate timer
        if (fireRateTimer > 0f)
        {
            fireRateTimer -= Time.deltaTime;
        }

        // Debugging: Draw a ray from the reflector to the focal point in the scene view
        if (focalPoint != null)
        {
            Debug.DrawLine(transform.position, focalPoint.position, Color.red);
        }
    }

    // Method to fire a projectile toward the focal point
    void FireProjectile()
    {
        if (projectilePrefab == null || focalPoint == null) return;

        // Calculate the direction from the reflector to the focal point
        Vector3 directionToFocalPoint = (focalPoint.position - transform.position).normalized;

        // Apply spawn offset along the direction towards the focal point
        Vector3 spawnPosition = transform.position + directionToFocalPoint * spawnOffset;

        // Instantiate the projectile at the spawn position
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Initialize the projectile using the TurretProjectile script
        TurretProjectile projScript = projectile.GetComponent<TurretProjectile>();
        if (projScript != null)
        {
            // Initialize the projectile to fire towards the focal point
            projScript.Initialize(null, directionToFocalPoint); // Pass null for turret, and the calculated direction
        }
        else
        {
            Debug.LogError("TurretProjectile script not found on instantiated projectile.");
        }
    }

    // This method handles what happens when the reflector is hit by a projectile
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only react to collisions with projectiles
        if (collision.gameObject.CompareTag("Projectile"))
        {
            // Destroy the incoming projectile
            Destroy(collision.gameObject);

            // Fire a new projectile from the reflector aimed at the focal point
            if (fireRateTimer <= 0f)
            {
                FireProjectile(); // Fire a new projectile towards the focal point
                fireRateTimer = fireRate; // Reset fire rate timer
            }
        }
    }
}
