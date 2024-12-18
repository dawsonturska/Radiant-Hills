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
    }

    private void OpenInventory()
    {
        // Show the inventory UI
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        // Optionally, trigger the onInteract event if needed
        onInteract?.Invoke();
    }

    // Method called when an item is selected from the inventory
    public void OnItemSelected(MaterialType selectedItem)
    {
        // Ensure we update the sprite only on the slot1 object
        if (displayShelf != null)
        {
            displayShelf.SetItem(selectedItem); // Update the DisplayShelf with the selected item
        }

        // Decrement the quantity of the item in the inventory
        if (playerInventory != null)
        {
            playerInventory.DecreaseQuantity(selectedItem, 1); // Decrease quantity by 1 (or adjust as needed)
        }

        // Close the inventory UI after the selection
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    // Detect when the player enters the interaction field (trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = true;
            Debug.Log("Player has entered the interaction field"); // Debug message
        }
    }

    // Detect when the player exits the interaction field (trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = false;
            Debug.Log("Player has exited the interaction field"); // Debug message
        }
    }
}
