using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractionField : MonoBehaviour
{
    [Header("Interaction Settings")]
    public UnityEvent onInteract; // Event to trigger on interaction (assignable in Inspector)
    public GameObject slot1; // Reference to Slot1 GameObject (set in Inspector)
    public GameObject inventoryPanel; // Reference to the player's inventory panel (set in Inspector)

    private bool isPlayerInRange = false; // Tracks if the player is in range to interact

    void Start()
    {
        // Ensure inventory panel is initially hidden
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player is in range and presses the E key to open inventory
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenInventory(); // Open the inventory when the player presses E
        }
    }

    private void OpenInventory()
    {
        // Trigger interaction event
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

    private void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // Hide the inventory UI
        }
    }
}
