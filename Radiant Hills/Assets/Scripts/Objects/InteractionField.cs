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

    // Reference to the IconGrid for dynamic interaction
    private IconGrid iconGrid;

    void Start()
    {
        // Initially set the IconGrid reference from SceneHandler if available
        iconGrid = SceneHandler.Instance.iconGrid;
        if (iconGrid == null)
        {
            Debug.LogWarning("IconGrid not found on Start.");
        }

        // Ensure inventory panel is initially hidden
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Inventory Panel is not assigned in the Inspector.");
        }

        if (slot1 == null)
        {
            Debug.LogWarning("Slot1 GameObject is not assigned in the Inspector.");
        }
    }

    // Method to set the IconGrid dynamically
    public void SetIconGrid(IconGrid newIconGrid)
    {
        if (newIconGrid != null)
        {
            iconGrid = newIconGrid;
            Debug.Log("IconGrid reference has been updated for InteractionField.");
        }
        else
        {
            Debug.LogWarning("Received null IconGrid reference.");
        }
    }

    void Update()
    {
        // Check if the player is in range and presses the E key to open inventory
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenInventory(); // Open the inventory when the player presses E
        }

        // Optional: Close inventory when the player presses the Escape key
        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory(); // Close the inventory when the player presses Escape
        }
    }

    private void OpenInventory()
    {
        // Trigger interaction event (if any)
        if (onInteract != null)
        {
            onInteract.Invoke();
            Debug.Log("Inventory Opened.");
        }
        else
        {
            Debug.LogWarning("No interact event assigned.");
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true); // Show the inventory UI
        }
    }

    // Detect when the player enters the interaction field (trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = true;
            Debug.Log("Player entered interaction range.");
        }
    }

    // Detect when the player exits the interaction field (trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = false;
            CloseInventory(); // Close the inventory when player exits the interaction field
            Debug.Log("Player exited interaction range.");
        }
    }

    private void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // Hide the inventory UI
            Debug.Log("Inventory Closed.");
        }
    }
}
