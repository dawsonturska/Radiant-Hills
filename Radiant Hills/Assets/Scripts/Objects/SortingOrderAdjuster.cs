using UnityEngine;

public class SortingOrderAdjuster : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject player; // Reference to the player GameObject
    private bool playerNotFound = true; // To ensure we log only once
    private Vector3 lastPlayerPosition; // To track player's last position

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If player is not assigned, try to find it dynamically
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Assuming the player has the "Player" tag
            if (player == null)
            {
                Debug.LogWarning("Player reference missing on Awake.");
            }
            else
            {
                Debug.Log("Player found in Awake.");
                lastPlayerPosition = player.transform.position; // Initialize last position
            }
        }
    }

    void Start()
    {
        // Check if player reference is still missing after Awake
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Try to find again in Start
            if (player == null)
            {
                Debug.LogWarning("Player reference missing on Start.");
            }
            else
            {
                Debug.Log("Player found in Start.");
                lastPlayerPosition = player.transform.position; // Initialize last position
            }
        }
    }

    void OnEnable()
    {
        // Additional check when the object is enabled
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing in SortingOrderAdjuster on enable.");
        }
        else
        {
            Debug.Log("Player found on enable.");
        }
    }

    void Update()
    {
        // If player reference is missing, we will log and try to find it again
        if (player == null && playerNotFound)
        {
           // Debug.LogWarning("Player reference missing in Update. Attempting to find player...");
            player = GameObject.FindWithTag("Player"); // Attempt to find again in Update
            playerNotFound = false; // Disable further logging

            // If player is found, initialize position
            if (player != null)
            {
                lastPlayerPosition = player.transform.position;
            }
        }

        // Only update sorting order if player has moved significantly
        if (player != null && Vector3.Distance(player.transform.position, lastPlayerPosition) > 0.1f)
        {
            UpdateSortingOrder();
            lastPlayerPosition = player.transform.position; // Update last position
        }
    }

    public void UpdateSortingOrder()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing in SortingOrderAdjuster.");
            return;
        }

        // Compare player's position to object's position
        if (player.transform.position.y > transform.position.y)
        {
            spriteRenderer.sortingOrder = 2; // Sorting Order for when player is above
        }
        else
        {
            spriteRenderer.sortingOrder = 0; // Sorting Order for when player is below
        }
    }
}
