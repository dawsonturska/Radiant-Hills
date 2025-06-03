using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttack : MonoBehaviour
{
    [Tooltip("Prefab for the attack effect")]
    public GameObject attackPrefab;

    [Tooltip("Cooldown time between attacks")]
    public float attackCooldown = 0.5f;

    [Tooltip("Time before the attack prefab is destroyed")]
    public float attackLifetime = 0.5f;

    [Tooltip("Attack offset in meters")]
    public float attackOffset = 1.524f; // (5 feet = ~1.524 meters)

    private Vector2 lastMoveDirection = Vector2.zero; // Last movement direction
    private float attackTimer = 0f;                  // Timer to handle attack cooldown

    private bool canAttack = true; // Flag to track whether the player can attack

    private PlayerMovement playerMovement;

    // List of scenes where the attack is disabled
    private readonly string[] attackDisabledScenes = { "Shop", "Town", "Scene3" };

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        CheckSceneForAttack();
    }

    private void Update()
    {
        // Handle cooldown timer
        attackTimer -= Time.deltaTime;

        // Update the last movement direction based on input
        UpdateMoveDirection();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Optional: Update canAttack if the scene changes during runtime
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneForAttack();
    }

    // Method to check if the player is in a scene where attack should be disabled
    private void CheckSceneForAttack()
    {
        // Get the current scene name
        string currentScene = SceneManager.GetActiveScene().name;

        // Disable attack if the current scene is in the disabled list
        canAttack = !System.Array.Exists(attackDisabledScenes, scene => scene == currentScene);

        if (!canAttack)
        {
            Debug.Log($"Attack is disabled in the scene: {currentScene}");
        }
    }

    private void UpdateMoveDirection()
    {
        Vector2 moveInput = playerMovement.GetMoveDirection();

        // Normalize input to ensure consistent directional values
        if (moveInput != Vector2.zero)
        {
            // Snap to 8 directions
            moveInput = SnapTo8Directions(moveInput.normalized);
            lastMoveDirection = moveInput;
        }
    }

    private Vector2 SnapTo8Directions(Vector2 input)
    {
        float x = Mathf.Round(input.x);
        float y = Mathf.Round(input.y);

        // Normalize diagonals to ensure consistent speed
        if (x != 0 && y != 0)
        {
            return new Vector2(x, y).normalized;
        }
        return new Vector2(x, y);
    }

    private void Attack()
    {
        if (lastMoveDirection == Vector2.zero)
        {
            Debug.Log("Player is stationary. No attack direction.");
            return;
        }

        // Calculate the spawn position offset
        Vector2 attackPosition = (Vector2)transform.position + lastMoveDirection * attackOffset;

        // Determine the rotation angle based on direction
        float rotationAngle = GetRotationAngle(lastMoveDirection);

        // Spawn attack effect at the calculated position with the correct rotation
        GameObject attackInstance = Instantiate(attackPrefab, attackPosition, Quaternion.Euler(0, 0, rotationAngle));
        attackInstance.transform.parent = transform; // Make it follow the player

        // Destroy the attack effect after the specified lifetime
        Destroy(attackInstance, attackLifetime);
    }

    private float GetRotationAngle(Vector2 direction)
    {
        // Check for cardinal directions
        if (direction == Vector2.up) return 0f;                     // Up
        if (direction == Vector2.right) return -90f;               // Right
        if (direction == Vector2.down) return 180f;                // Down
        if (direction == Vector2.left) return 90f;                 // Left

        // Check for diagonal directions
        if (direction == (Vector2.up + Vector2.right).normalized) return -45f;  // Up-Right
        if (direction == (Vector2.down + Vector2.right).normalized) return -135f; // Down-Right
        if (direction == (Vector2.down + Vector2.left).normalized) return 135f;   // Down-Left
        if (direction == (Vector2.up + Vector2.left).normalized) return 45f;      // Up-Left

        return 0f; // Default to facing up
    }

    /// <summary>
    /// Handle player attack given context.performed
    /// </summary>
    /// <param name="context"></param>
    public void HandleAttack(InputAction.CallbackContext context)
    {
        if (canAttack && attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }
}