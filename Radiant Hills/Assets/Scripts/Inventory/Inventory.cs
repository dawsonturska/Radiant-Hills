using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour, IInteractable
{
    public static Inventory Instance { get; private set; }

    [Tooltip("Centralized database of materials")]
    public MaterialDatabase materialDatabase;

    [Tooltip("Inventory items consist of Material and quantity, keyed by MaterialType")]
    public Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>();

    [Tooltip("Reference to IconGrid UI element")]
    public IconGrid iconGrid;

    [Tooltip("Currently targeted DisplayShelf")]
    public DisplayShelf displayShelf;

    public delegate void SlotClickHandler(int slotIndex);
    public event SlotClickHandler OnSlotClicked;

    private GameObject panel;
    private bool isGridOpen = false;

    private Transform player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (iconGrid != null) DontDestroyOnLoad(iconGrid.gameObject);
        if (displayShelf != null) DontDestroyOnLoad(displayShelf.gameObject);
    }

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (iconGrid != null) iconGrid.gameObject.SetActive(false);
    }

    // If shelf has item, add to inventory
    private void TryPickupItemFromShelf()
    {
        if (displayShelf != null && displayShelf.HasItem())
        {
            MaterialType material = displayShelf.GetItem();
            if (material == null) return;

            AddMaterial(material, 1);
            displayShelf.ClearItem();
        }
    }

    /// <summary>
    /// Toggle whether inventory is visible
    /// </summary>
    public void ToggleInventoryVisibility()
    {
        isGridOpen = !isGridOpen;
        if (panel != null) panel.SetActive(isGridOpen);
        if (iconGrid != null) iconGrid.gameObject.SetActive(isGridOpen);
        Debug.Log($"Inventory {(isGridOpen ? "Closed" : "Opened")}");
    }

    public void CloseInventory()
    {
        if (IsInventoryVisible) ToggleInventoryVisibility();
    }

    public bool IsInventoryVisible => isGridOpen;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        Debug.Log("Player reference set.");
    }

    #region MANAGE MATERIALS IN INVENTORY
    /// <summary>
    /// Add quantity of MaterialType to inventory
    /// </summary>
    public void AddMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null) return;

        if (materialQuantities.ContainsKey(materialType))
            materialQuantities[materialType] += quantity;
        else
            materialQuantities[materialType] = quantity;

        iconGrid?.PopulateGrid();
    }

    /// <summary>
    /// Remove quantity of MaterialType from inventory
    /// </summary>
    public void RemoveMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null) return;

        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType] -= quantity;
            if (materialQuantities[materialType] <= 0)
                materialQuantities.Remove(materialType);
        }

        iconGrid?.PopulateGrid();
    }

    /// <summary>
    /// Returns true if inventory contains MaterialType more than given quantity
    /// </summary>
    public bool HasMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null) return false;

        return materialQuantities.ContainsKey(materialType) && materialQuantities[materialType] >= quantity;
    }
    #endregion

    #region INVENTORY DATA MANAGERS

    /// <summary>
    /// Save Inventory as JSON
    /// </summary>
    public void SaveInventory()
    {
        // Convert inventory to SerializableInventory
        SerializableInventory serializableInventory = new SerializableInventory();

        foreach (var item in materialQuantities)
        {
            InventoryItem inventoryItem = new InventoryItem
            {
                materialName = item.Key.materialName,
                quantity = item.Value
            };

            serializableInventory.items.Add(inventoryItem);
        }

        // Serialize the inventory to JSON
        string json = JsonUtility.ToJson(serializableInventory, true);

        // Save JSON to file (or PlayerPrefs, if needed)
        string path = Application.persistentDataPath + "/inventory.json";
        File.WriteAllText(path, json);

        Debug.Log("Inventory saved to: " + path); // Log the path for debugging

        // WHY IS INVENTORY SAVING PLAYER POSIION??
        // Optionally save the player position
        if (player != null)
        {
            Vector3 pos = player.position;
            PlayerPrefs.SetFloat("PlayerPosX", pos.x);
            PlayerPrefs.SetFloat("PlayerPosY", pos.y);
            PlayerPrefs.SetFloat("PlayerPosZ", pos.z);
        }
    }

    /// <summary>
    /// Load Inventory from JSON
    /// </summary>
    public void LoadInventory()
    {
        string path = Application.persistentDataPath + "/inventory.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Loaded inventory JSON: " + json);  // Debug line to check the loaded JSON
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);

            materialQuantities.Clear(); // Clear old inventory data
            Debug.Log("Loaded " + inventoryData.items.Count + " items."); // Check how many items were loaded

            foreach (var item in inventoryData.items)
            {
                MaterialType material = materialDatabase.FindMaterialByName(item.materialName);
                if (material != null)
                {
                    // Add the material to the inventory
                    AddMaterial(material, item.quantity);
                    Debug.Log($"Added {material.materialName} with quantity {item.quantity}."); // Debug line to confirm the addition
                }
                else
                {
                    Debug.LogWarning($"Material '{item.materialName}' not found in 'allMaterials'.");
                }
            }

            // Refresh the inventory UI after loading
            if (iconGrid != null)
            {
                iconGrid.PopulateGrid();  // Repopulate the grid with the updated inventory data
            }
            else
            {
                Debug.LogWarning("IconGrid is not assigned!");
            }
        }
        else
        {
            Debug.LogError("No inventory file found at: " + path);
        }

        // Ensure the icon grid is populated after loading
        iconGrid?.PopulateGrid();  // Refresh the UI (this is redundant, remove if the first call works)
    }

    /// <summary>
    /// Clear all items from inventory and clear inventory JSON
    /// </summary>
    public void ClearInventory()
    {
        materialQuantities.Clear();

        // Refresh the UI
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid();
        }

        // Optionally clear the saved file
        string path = Application.persistentDataPath + "/inventory.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Inventory file deleted.");
        }

        Debug.Log("Inventory has been cleared.");
    }
    #endregion

    /// <summary>
    /// Handler for "Inventory" action
    /// </summary>
    public void HandleToggleInventory(InputAction.CallbackContext context)
    {
        ToggleInventoryVisibility();
    }

    /// <summary>
    /// Handler for "Interact" action
    /// </summary>
    public void Interact(PlayerInputHandler handler)
    {
        TryPickupItemFromShelf();
    }
}