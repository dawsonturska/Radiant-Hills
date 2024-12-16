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

    // References to player health
    private PlayerHealth playerHealth;

    void Start()
    {
        player = GameObject.FindWithTag("Player"); // Find player by tag
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>(); // Get the PlayerHealth component
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= aggroRange && !isInAggroRange)
            {
                isInAggroRange = true;
                StartCoroutine(WaitBeforeMoving());
            }

            if (isInAggroRange && canMove)
            {
                MoveTowardsPlayer();
            }
        }
    }

    private IEnumerator WaitBeforeMoving()
    {
        yield return new WaitForSeconds(aggroDelay);
        canMove = true;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;

        if (direction.magnitude > minDistance)
        {
            Vector3 moveDirection = direction.normalized * moveSpeed * Time.deltaTime;
            transform.position += moveDirection;
        }
    }

    public void TakeDamage(int damage)
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
            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                int dropQuantity = Random.Range(minDropQuantity, maxDropQuantity + 1);
                inventory.AddMaterial(materialDrop, dropQuantity);
                Debug.Log($"Dropped {dropQuantity} {materialDrop.materialName}.");
            }
            else
            {
                Debug.LogError("Player does not have an Inventory component!");
            }
        }
        else
        {
            Debug.LogWarning("MaterialType is not set for this enemy.");
        }
    }

    // Change from OnTriggerEnter2D to OnCollisionEnter2D for non-trigger colliders
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy, applying damage.");
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f); // Adjust damage as needed
            }
        }
    }
}
