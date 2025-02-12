using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; } // Singleton pattern for Inventory instance

    public delegate void SlotClickHandler(int slotIndex); // Delegate for slot click events
    public event SlotClickHandler OnSlotClicked; // Event to subscribe to slot clicks

    public Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>(); // Dictionary holding material types and their quantities
    public IconGrid iconGrid; // Reference to the IconGrid (set this in the Inspector)
    public DisplayShelf displayShelf; // Reference to the DisplayShelf (set this in the Inspector)
    private GameObject panel; // Reference to the parent panel (where IconGrid is)
    private bool isGridOpen = false; // Whether the grid is open or closed

    private Transform player; // Reference to the player object

    void Awake()
    {
        // Singleton pattern to ensure only one instance of Inventory exists
        if (Instance == null)
        {
            Instance = this; // Set this as the instance
            DontDestroyOnLoad(gameObject); // Prevent destruction on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        // Ensure IconGrid and DisplayShelf are not destroyed on scene load
        if (iconGrid != null)
        {
            DontDestroyOnLoad(iconGrid.gameObject);
        }

        if (displayShelf != null)
        {
            DontDestroyOnLoad(displayShelf.gameObject);
        }
    }

    void Start()
    {
        // Ensure the panel and IconGrid are inactive at the start
        if (panel != null)
        {
            panel.SetActive(false); // Make the inventory panel invisible by default
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(false); // Make the IconGrid invisible by default
        }
    }

    void Update()
    {
        // Check for the I key press to toggle inventory visibility
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryVisibility(); // Toggle inventory on/off
        }

        // Check for the E key press to pick up an item from the shelf
        if (Input.GetKeyDown(KeyCode.E) && displayShelf != null)
        {
            TryPickupItemFromShelf(); // Attempt to pick up item from the display shelf
        }
    }

    // Toggle visibility of the inventory grid
    public void ToggleInventoryVisibility()
    {
        isGridOpen = !isGridOpen; // Toggle the flag

        if (panel != null)
        {
            panel.SetActive(isGridOpen); // Toggle visibility of the inventory panel
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(isGridOpen); // Toggle visibility of the IconGrid
        }
    }

    // Property to check if the inventory is visible
    public bool IsInventoryVisible
    {
        get { return isGridOpen; }
    }

    // Set the player reference in the inventory
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    // Add material to the inventory
    public void AddMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null)
        {
            Debug.LogError("MaterialType is null. Cannot add to inventory.");
            return;
        }

        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType] += quantity; // Update quantity if material exists
        }
        else
        {
            materialQuantities[materialType] = quantity; // Add new material to inventory
        }

        // Refresh the grid after adding material
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid(); // Calls the PopulateGrid method to update the inventory UI
        }

        Debug.Log($"Added {quantity} of {materialType.materialName} to inventory.");
    }

    // Remove material from the inventory
    public void RemoveMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null)
        {
            Debug.LogError("MaterialType is null. Cannot remove from inventory.");
            return;
        }

        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType] -= quantity; // Decrease the material quantity
            if (materialQuantities[materialType] <= 0)
            {
                materialQuantities.Remove(materialType); // Remove material if quantity is zero
            }
        }
        else
        {
            Debug.LogWarning($"Material {materialType.materialName} not found in inventory.");
        }

        // Refresh the grid after removing material
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid(); // Calls the PopulateGrid method to update the inventory UI
        }

        Debug.Log($"Removed {quantity} of {materialType.materialName} from inventory.");
    }

    // Check if the player has enough material in the inventory
    public bool HasMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null)
        {
            Debug.LogError("MaterialType is null. Cannot check material in inventory.");
            return false;
        }

        return materialQuantities.ContainsKey(materialType) && materialQuantities[materialType] >= quantity;
    }

    // Attempt to pick up an item from the display shelf
    private void TryPickupItemFromShelf()
    {
        if (displayShelf != null && displayShelf.HasItem()) // Check if the shelf has an item
        {
            MaterialType material = displayShelf.GetItem(); // Get the item from the shelf

            if (material == null)
            {
                Debug.LogError("MaterialType is null. Could not pick up item from shelf.");
                return;
            }

            if (HasMaterial(material, 1)) // Ensure player has space
            {
                // Add the item to the inventory
                AddMaterial(material, 1);

                // Clear the item from the shelf (so it becomes empty)
                displayShelf.ClearItem();
                Debug.Log($"Picked up item: {material.materialName}");
            }
            else
            {
                Debug.LogWarning("Not enough space in inventory to pick up this item.");
            }
        }
        else
        {
            Debug.Log("No item on the display shelf to pick up.");
        }
    }
}
