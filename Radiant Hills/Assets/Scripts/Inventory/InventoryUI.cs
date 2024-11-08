using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;  // Reference to the player's inventory (assign via Inspector)
    public GameObject inventoryPanel;  // Reference to the panel that holds inventory slots
    public GameObject inventorySlotTemplate;  // The template for inventory slots (prefab)

    private void Start()
    {
        UpdateInventoryUI();  // Call the UI update when the game starts
    }

    public void UpdateInventoryUI()
    {
        // Check if inventoryPanel and inventorySlotTemplate are set up correctly
        if (inventoryPanel == null || inventorySlotTemplate == null || playerInventory == null)
        {
            Debug.LogError("Inventory Panel, Slot Template, or Player Inventory is not assigned in the Inspector!");
            return; // Early exit if references are missing
        }

        // Clear previous inventory slots
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Loop through each item in the player's inventory and instantiate a UI slot for it
        foreach (Item item in playerInventory.items)
        {
            // Instantiate the slot only if the template is not null
            GameObject slot = Instantiate(inventorySlotTemplate, inventoryPanel.transform); // Create a new slot
            slot.SetActive(true); // Ensure the slot is visible

            // Get the Image component within the slot (for the item icon)
            Image itemIcon = slot.GetComponentInChildren<Image>();  // Assuming your template has an Image for the item icon

            // Get the Text component within the slot (for the item name and quantity)
            Text itemText = slot.GetComponentInChildren<Text>();   // Assuming your template has a Text component for item name

            // If the item has an icon, set it
            if (itemIcon != null && item.itemIcon != null)
            {
                itemIcon.sprite = item.itemIcon;
            }

            // If the Text component is found, set the item name and quantity
            if (itemText != null)
            {
                itemText.text = $"{item.itemName} x{item.quantity}";
            }
            else
            {
                Debug.LogError("Text component is missing from the inventory slot prefab!");
            }

            Debug.Log("Added item to UI: " + item.itemName);  // Log the item added to UI
        }
    }
}