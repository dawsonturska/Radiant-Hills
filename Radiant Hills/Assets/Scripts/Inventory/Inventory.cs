using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public delegate void SlotClickHandler(int slotIndex); // Define the delegate for slot click events
    public event SlotClickHandler OnSlotClicked;  // Event to subscribe to slot clicks

    public Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>();  // Tracks quantities
    public IconGrid iconGrid; // Reference to the IconGrid (set this in the Inspector)
    private bool isGridOpen = false; // Whether the grid is open or closed
    private GameObject panel; // Reference to the parent panel (where IconGrid is)

    void Awake()
    {
        if (iconGrid != null)
        {
            panel = iconGrid.transform.parent.gameObject; // Get the parent GameObject (panel)
        }
    }

    void Start()
    {
        // Set the inventory panel and IconGrid to be inactive at the start
        if (panel != null)
        {
            panel.SetActive(false); // Make the inventory panel invisible by default
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(false); // Make the IconGrid invisible by default
        }
    }

    // Adds a material to the inventory
    public void AddMaterial(MaterialType materialType, int quantity)
    {
        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType] += quantity; // Add the specified quantity
        }
        else
        {
            materialQuantities[materialType] = quantity; // Initialize with the specified quantity
        }

        // Debugging: Log when material is added
        Debug.Log($"Material added: {materialType.name}, Quantity: {materialQuantities[materialType]}");

        // Ensure that the grid is populated again after adding the material
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid(); // Update the grid with the new inventory state
        }
        else
        {
            Debug.LogWarning("IconGrid is not assigned.");
        }
    }

    // Get item by slot index
    public MaterialType GetItemBySlotIndex(int slotIndex)
    {
        // Assuming you store items in a list based on the index:
        List<MaterialType> allItems = new List<MaterialType>(materialQuantities.Keys);
        if (slotIndex >= 0 && slotIndex < allItems.Count)
        {
            return allItems[slotIndex]; // Return the item at the given index
        }
        return null;
    }

    // Decrease quantity of an item in the inventory
    public void DecreaseQuantity(MaterialType materialType, int amount)
    {
        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType] -= amount; // Decrease the quantity

            if (materialQuantities[materialType] <= 0)
            {
                materialQuantities.Remove(materialType); // Remove the item if the quantity is 0 or less
            }

            // Optionally, update the UI grid or anything else after decrementing
            if (iconGrid != null)
            {
                iconGrid.PopulateGrid(); // Refresh the grid
            }
        }
        else
        {
            Debug.LogWarning($"Material {materialType.name} not found in inventory.");
        }
    }

    // Update method to handle key press for toggling inventory grid visibility
    private void Update()
    {
        // Toggle visibility of the inventory panel and IconGrid on "I" key press
        if (Input.GetKeyDown(KeyCode.I))
        {
            isGridOpen = !isGridOpen;

            // Update grid visibility
            if (panel != null)
            {
                panel.SetActive(isGridOpen); // Show or hide the inventory panel
            }

            if (iconGrid != null)
            {
                iconGrid.gameObject.SetActive(isGridOpen); // Show or hide the IconGrid

                // Only refresh the grid when it's opened and hasn't been refreshed recently
                if (isGridOpen && !iconGrid.IsGridPopulated)
                {
                    iconGrid.PopulateGrid(); // Refresh the grid if not already populated
                }
            }
        }
    }

    // Method to trigger the slot click event when a UI slot is clicked
    public void OnInventorySlotClicked(int slotIndex)
    {
        // Invoke the slot click event to notify listeners
        OnSlotClicked?.Invoke(slotIndex);
    }
}
