using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    public string targetScene; // Name of the scene to load
    public float holdDuration = 3f; // Time in seconds the player needs to hold the key
    public Vector3 spawnPosition = Vector3.zero; // Customizable spawn position in the inspector
    private float holdTimer = 0f; // Tracks how long the player has been holding the key
    private bool isPlayerInRange = false; // Tracks if the player is in the portal's range
    private GameObject player; // Reference to the player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                playerHandler.SetCurrentInteractable(this);
                isPlayerInRange = true;
                player = collision.gameObject; // Store reference to player
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                playerHandler.ClearInteractable(this);
                isPlayerInRange = false;
                holdTimer = 0f; // Reset the timer when the player leaves the portal
                player = null; // Clear reference to player
            }
        }
    }

    private void TeleportPlayer()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event
            SceneManager.LoadScene(targetScene); // Load the target scene
        }
        else
        {
            Debug.LogError("Target scene is not set in the Portal script!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); // Find the player if it's marked as DontDestroyOnLoad
        }

        if (player != null)
        {
            player.transform.position = spawnPosition; // Set player position to the defined spawn position
        }

        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent multiple calls
    }

    /// <summary>
    /// Handler for "Interact" action
    /// </summary>
    public void Interact(PlayerInputHandler handler)
    {
        if (isPlayerInRange)
        {
            holdTimer += Time.deltaTime; // Increment the timer

            if (holdTimer >= holdDuration)
            {
                TeleportPlayer();
            }
            else
            {
                holdTimer = 0f; // Reset the timer if the key is released
            }
        }
    }
}
