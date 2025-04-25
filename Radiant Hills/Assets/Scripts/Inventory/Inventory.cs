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

    // Predefined list of materials available in the game (used for loading inventory without MaterialDatabase)
    public List<MaterialType> allMaterials; // Reference to a list of all possible materials

    void Awake()
    {
        // Singleton pattern to ensure only one instance of Inventory exists
        if (Instance == null)
        {
            Instance = this; // Set this as the instance
            DontDestroyOnLoad(gameObject); // Prevent destruction on scene load
            Debug.Log("Inventory instance created.");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            Debug.Log("Destroyed duplicate Inventory instance.");
        }

        // Ensure IconGrid and DisplayShelf are not destroyed on scene load
        if (iconGrid != null)
        {
            DontDestroyOnLoad(iconGrid.gameObject);
            Debug.Log("IconGrid will not be destroyed on scene load.");
        }

        if (displayShelf != null)
        {
            DontDestroyOnLoad(displayShelf.gameObject);
            Debug.Log("DisplayShelf will not be destroyed on scene load.");
        }
    }

    void Start()
    {
        // Ensure the panel and IconGrid are inactive at the start
        if (panel != null)
        {
            panel.SetActive(false); // Make the inventory panel invisible by default
            Debug.Log("Inventory panel set to inactive at start.");
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(false); // Make the IconGrid invisible by default
            Debug.Log("IconGrid set to inactive at start.");
        }
    }

    void Update()
    {
        // Prevent input while the game is paused
        if (Time.timeScale == 0)
            return;

        // Check for the I key press to toggle inventory visibility
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryVisibility(); // Toggle inventory on/off
            Debug.Log("Toggled inventory visibility.");
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
            Debug.Log($"Inventory panel visibility toggled: {isGridOpen}");
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(isGridOpen); // Toggle visibility of the IconGrid
            Debug.Log($"IconGrid visibility toggled: {isGridOpen}");
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
        Debug.Log("Player reference set.");
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
            Debug.Log($"Updated {materialType.materialName} quantity to {materialQuantities[materialType]}.");
        }
        else
        {
            materialQuantities[materialType] = quantity; // Add new material to inventory
            Debug.Log($"Added new material: {materialType.materialName} with quantity {quantity}.");
        }

        // Refresh the grid after adding material
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid(); // Calls the PopulateGrid method to update the inventory UI
            Debug.Log("Inventory grid populated after adding material.");
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
                Debug.Log($"Material {materialType.materialName} removed from inventory due to zero quantity.");
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
            Debug.Log("Inventory grid populated after removing material.");
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

        bool hasMaterial = materialQuantities.ContainsKey(materialType) && materialQuantities[materialType] >= quantity;
        Debug.Log($"Check if inventory has {quantity} of {materialType.materialName}: {hasMaterial}");
        return hasMaterial;
    }

    // Attempt to pick up an item from the display shelf
    public void TryPickupItemFromShelf()
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
                Debug.Log($"Picked up item: {material.materialName} from shelf.");
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

    // Save inventory data to persistent storage (e.g., PlayerPrefs or a file)
    public void SaveInventory()
    {
        int count = 0;
        foreach (var item in materialQuantities)
        {
            PlayerPrefs.SetString($"InventoryItem_{count}_Name", item.Key.materialName);
            PlayerPrefs.SetInt($"InventoryItem_{count}_Quantity", item.Value);
            count++;
            Debug.Log($"Saving material: {item.Key.materialName} with quantity {item.Value}.");
        }
        PlayerPrefs.SetInt("InventoryCount", count);
        PlayerPrefs.Save();

        Debug.Log($"Saved {count} items to inventory.");
    }

    public void LoadInventory()
    {
        int itemCount = PlayerPrefs.GetInt("InventoryCount", 0);
        //materialQuantities.Clear();  // Clear any previously stored data

        Debug.Log($"Loading {itemCount} items from inventory.");

        for (int i = 0; i < itemCount; i++)
        {
            string materialName = PlayerPrefs.GetString($"InventoryItem_{i}_Name", string.Empty);
            if (!string.IsNullOrEmpty(materialName))
            {
                MaterialType material = FindMaterialByName(materialName);
                int quantity = PlayerPrefs.GetInt($"InventoryItem_{i}_Quantity", 0);

                if (material != null && quantity > 0)
                {
                    AddMaterial(material, quantity);
                    Debug.Log($"Loaded {materialName} with quantity {quantity}.");
                }
                else
                {
                    Debug.LogWarning($"Material '{materialName}' not found or quantity is zero.");
                }
            }
        }

        // Ensure the UI is updated after loading data
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid();  // This will refresh the inventory UI
            Debug.Log("Inventory grid repopulated after loading.");
        }
    }

    // Find material by name from the list of all available materials
    private MaterialType FindMaterialByName(string name)
    {
        foreach (MaterialType material in allMaterials)
        {
            if (material.materialName == name)
            {
                Debug.Log($"Found material: {name}");
                return material;
            }
        }
        Debug.LogWarning($"Material '{name}' not found.");
        return null;
    }
}
