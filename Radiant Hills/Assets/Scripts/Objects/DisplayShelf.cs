using UnityEngine;

public class DisplayShelf : MonoBehaviour
{
    public SpriteRenderer itemSpriteRenderer; // The SpriteRenderer on the DisplayShelf (child of slot1)
    public MaterialType selectedItem; // The selected item for the shelf slot
    public Inventory inventory; // Reference to the inventory manager (set in Inspector)

    private GameObject inventoryPanel; // Panel showing the inventory grid

    void Start()
    {
        if (itemSpriteRenderer == null)
        {
            itemSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (inventory != null)
        {
            inventoryPanel = inventory.gameObject; // Assuming Inventory has a GameObject with the panel
        }

        UpdateItemSprite();
    }

    // This method updates the sprite of the parent (slot1) based on the selected item
    public void UpdateItemSprite()
    {
        if (selectedItem != null && selectedItem.icon != null)
        {
            // Access the slot1 GameObject directly and its SpriteRenderer
            SpriteRenderer slot1SpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();

            if (slot1SpriteRenderer != null)
            {
                // Update the sprite of slot1 (not the parent shelfr1)
                slot1SpriteRenderer.sprite = selectedItem.icon;
            }
            else
            {
                Debug.LogError("Slot1 does not have a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Selected item or its icon is missing.");
        }
    }

    // Optional: Update the selected item when an item is placed into this slot
    public void SetItem(MaterialType item)
    {
        selectedItem = item;
        UpdateItemSprite();  // Update the sprite when the item is set
    }

    // Display the inventory and listen for slot clicks
    public void DisplayItemFromSlot(int slotIndex)
    {
        // Open the inventory UI
        if (inventory != null && inventoryPanel != null)
        {
            inventoryPanel.SetActive(true); // Show the inventory panel
        }
        else
        {
            Debug.LogWarning("Inventory or panel is not assigned.");
        }
    }
}
