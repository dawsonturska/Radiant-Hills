using UnityEngine;
using UnityEngine.UI; // For Unity UI events (like button clicks)

public class DisplayShelf : MonoBehaviour
{
    public SpriteRenderer itemDisplaySpriteRenderer; // Sprite that shows the material icon
    private MaterialType currentMaterial; // Currently stored material
    public Sprite emptySprite; // Default empty sprite for the shelf
    private bool isPlayerInRange = false; // Whether the player is within the collider
    private Transform playerTransform; // Player's transform
    private Inventory inventory; // Reference to the player's inventory
    private IconGrid iconGrid; // Reference to the IconGrid (where items are displayed)

    public GameObject inventoryPanel; // Inventory panel to toggle visibility
    private BoxCollider2D interactionCollider; // BoxCollider2D for triggering interaction

    void Start()
    {
        // Initialize references and colliders
        Initialize();
    }

    // Initialize necessary references and components
    public void Initialize()
    {
        // Cache the references to the Inventory and IconGrid objects
        inventory = FindObjectOfType<Inventory>(); // Ensures each shelf has access to the inventory
        iconGrid = FindObjectOfType<IconGrid>(); // Each shelf gets its own grid

        // Set up the box collider 2 units below the sprite renderer (adjust size and position as needed)
        interactionCollider = gameObject.AddComponent<BoxCollider2D>();
        interactionCollider.size = new Vector2(itemDisplaySpriteRenderer.bounds.size.x, 1); // Adjust size if needed
        interactionCollider.offset = new Vector2(0, -itemDisplaySpriteRenderer.bounds.extents.y - 1); // Offset by 1 unit below sprite
        interactionCollider.isTrigger = true; // Set to trigger to avoid collision with player

        playerTransform = Camera.main.transform; // Assuming the camera is attached to the player

        // Make sure the inventory panel is hidden by default
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Open/close the inventory when player presses 'E' (if within range)
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // If there is an item on the shelf, allow the player to pick it up
            if (HasItem())
            {
                PickupItem();
            }
            else
            {
                ToggleInventory(); // Otherwise, toggle the inventory
            }
        }

        // Close the inventory if Escape key is pressed
        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    // Toggles the visibility of the inventory panel
    private void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isOpen = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isOpen);
            Debug.Log(isOpen ? "Inventory Closed." : "Inventory Opened.");
        }
    }

    // Closes the inventory panel
    private void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    // Handles when the player enters the interaction zone (collider)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    // Handles when the player leaves the interaction zone (collider)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseInventory(); // Close inventory when the player exits range
        }
    }

    // Stores an item from the inventory into the shelf
    public void StoreItemInShelf(MaterialType material)
    {
        if (material == null)
        {
            Debug.LogWarning("Trying to store a null material.");
            return;
        }

        // Store the item on the shelf
        currentMaterial = material;
        SetItemIcon(material); // Update the icon on the shelf
        inventory.RemoveMaterial(material, 1); // Remove one item of this material from the inventory
    }

    // Updates the shelf's sprite based on the stored material
    private void SetItemIcon(MaterialType material)
    {
        if (itemDisplaySpriteRenderer == null)
        {
            Debug.LogWarning("Item display sprite renderer is not assigned.");
            return;
        }

        // Set the sprite to the material's icon, or use the empty sprite if no icon is assigned
        itemDisplaySpriteRenderer.sprite = material.icon != null ? material.icon : emptySprite;
    }

    // Clears the item from the shelf and resets the display
    public void ClearItem()
    {
        currentMaterial = null;
        itemDisplaySpriteRenderer.sprite = emptySprite;
    }

    // Returns true if the shelf currently has an item
    public bool HasItem()
    {
        return currentMaterial != null;
    }

    // Returns whether the player is within range of the shelf for interaction
    public bool IsPlayerInRange()
    {
        return isPlayerInRange;
    }

    // Called when an item is clicked in the inventory (stores the selected material on the shelf)
    public void OnItemClicked(MaterialType material)
    {
        if (material != null)
        {
            StoreItemInShelf(material); // Store the selected material in the shelf
            CloseInventory(); // Optionally close the inventory after storing
        }
    }

    // Gets the material stored in the shelf
    public MaterialType GetItem()
    {
        return currentMaterial;
    }

    // Pick up the item and add it to the inventory
    private void PickupItem()
    {
        // If there is a material on the shelf
        if (HasItem())
        {
            // Add the item to the inventory
            inventory.AddMaterial(currentMaterial, 1); // Add one of the stored item to inventory
            Debug.Log("Picked up: " + currentMaterial.name);

            // Clear the shelf after picking up the item
            ClearItem();
        }
    }
}
