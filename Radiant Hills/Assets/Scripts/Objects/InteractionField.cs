using UnityEngine;
using UnityEngine.Events;

public class InteractionField : MonoBehaviour
{
    [Header("Interaction Settings")]
    public UnityEvent onInteract; // Event to trigger on interaction (assignable in Inspector)
    public GameObject slot1; // Reference to Slot1 GameObject (set in Inspector)
    public GameObject inventoryPanel; // Reference to the player's inventory panel (set in Inspector)

    private bool isPlayerInRange = false; // Tracks if the player is in range to interact
    private DisplayShelf displayShelf; // Cached reference to the DisplayShelf component
    private Inventory playerInventory; // Reference to the player's inventory

    void Start()
    {
        // Cache the DisplayShelf reference from Slot1 directly
        if (slot1 != null)
        {
            displayShelf = slot1.GetComponent<DisplayShelf>(); // Directly get DisplayShelf from Slot1
            if (displayShelf == null)
            {
                Debug.LogWarning("No DisplayShelf component found on Slot1.");
            }
        }

        // Cache the player inventory reference
        playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogWarning("Player inventory not found.");
        }

        // Ensure the inventory panel is initially hidden
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player is in range and presses the E key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenInventory(); // Open the inventory when the player presses E
        }

        // Check if the player presses the I key at any time (independent of interaction)
        if (Input.GetKeyDown(KeyCode.I))
        {
            playerInventory.ToggleInventoryVisibility(); // Toggle the inventory visibility
        }
    }

    private void OpenInventory()
    {
        // Show the inventory UI if not already opened
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        // Open the inventory when the player interacts with the shelf
        playerInventory.ToggleInventoryVisibility(); // Open or close the inventory as per usual functionality

        // Optionally, trigger the onInteract event if needed
        onInteract?.Invoke();
    }

    // Detect when the player enters the interaction field (trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = true;
        }
    }

    // Detect when the player exits the interaction field (trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = false;
            CloseInventory(); // Close the inventory when player exits the interaction field
        }
    }

    // Close the inventory window
    private void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // Hide the inventory UI
        }

        // Close the inventory using the player's inventory script
        if (playerInventory != null)
        {
            playerInventory.ToggleInventoryVisibility(); // Ensure the inventory is closed
        }
    }
}
