using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health = 100; // Enemy's health
    public MaterialType materialDrop; // Material dropped on death
    public int minDropQuantity = 1; // Minimum quantity of the material to drop
    public int maxDropQuantity = 5; // Maximum quantity of the material to drop

    public float moveSpeed = 3f; // Speed at which the enemy moves
    public float minDistance = 0.5f; // Minimum distance from player before stopping
    public float aggroRange = 10f; // Distance at which the enemy starts moving towards the player
    public float aggroDelay = 1f; // Delay in seconds after player enters aggro range before enemy starts moving

    private GameObject player; // Reference to the player object
    private bool isInAggroRange = false; // Flag to track if the player is in aggro range
    private bool canMove = true; // Flag to track if the enemy can move

    private void Start()
    {
        player = GameObject.FindWithTag("Player"); // Find player by tag
    }

    private void Update()
    {
        if (player != null)
        {
            // Calculate the distance from the enemy to the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Check if the player is within the aggro range
            if (distanceToPlayer <= aggroRange && !isInAggroRange)
            {
                // Player has just entered aggro range, start the aggro delay
                isInAggroRange = true;
                StartCoroutine(WaitBeforeMoving());
            }

            // If the player is in aggro range and movement is allowed, move towards the player
            if (isInAggroRange && canMove)
            {
                MoveTowardsPlayer();
            }
        }
    }

    private IEnumerator WaitBeforeMoving()
    {
        // Wait for the aggro delay before allowing movement
        yield return new WaitForSeconds(aggroDelay);
        canMove = true; // Enable movement after delay
    }

    private void MoveTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector3 direction = player.transform.position - transform.position;

        // If the distance to the player is greater than the minimum allowed distance
        if (direction.magnitude > minDistance)
        {
            // Normalize the direction vector and move towards the player
            Vector3 moveDirection = direction.normalized * moveSpeed * Time.deltaTime;

            // Move the enemy towards the player
            transform.position += moveDirection;
        }
    }

    public void TakeDamage(int damage) // Changed to public
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DropMaterial();
        Destroy(gameObject);
    }

    private void DropMaterial()
    {
        if (materialDrop != null)
        {
            if (player != null)
            {
                Inventory inventory = player.GetComponent<Inventory>();

                if (inventory != null)
                {
                    int dropQuantity = Random.Range(minDropQuantity, maxDropQuantity + 1); // Generate random drop quantity
                    for (int i = 0; i < dropQuantity; i++)
                    {
                        inventory.AddMaterial(materialDrop);
                    }

                    Debug.Log($"Dropped {dropQuantity} {materialDrop.materialName}.");
                }
                else
                {
                    Debug.LogError("Player does not have an Inventory component!");
                }
            }
        }
        else
        {
            Debug.LogWarning("MaterialType is not set for this enemy.");
        }
    }
}
