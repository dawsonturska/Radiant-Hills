using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public delegate void SlotClickHandler(int slotIndex);
    public event SlotClickHandler OnSlotClicked;

    public Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>();
    public IconGrid iconGrid;
    public DisplayShelf displayShelf;
    private GameObject panel;
    private bool isGridOpen = false;
    private Transform player;

    public List<MaterialType> allMaterials;  // Make sure this list is populated with all available materials.

    void Awake()
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

    void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (iconGrid != null) iconGrid.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        if (Input.GetKeyDown(KeyCode.I)) ToggleInventoryVisibility();
        if (Input.GetKeyDown(KeyCode.E) && displayShelf != null) TryPickupItemFromShelf();
    }

    public void ToggleInventoryVisibility()
    {
        isGridOpen = !isGridOpen;
        if (panel != null) panel.SetActive(isGridOpen);
        if (iconGrid != null) iconGrid.gameObject.SetActive(isGridOpen);
    }

    public bool IsInventoryVisible => isGridOpen;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        Debug.Log("Player reference set.");
    }

    public void AddMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null) return;

        if (materialQuantities.ContainsKey(materialType))
            materialQuantities[materialType] += quantity;
        else
            materialQuantities[materialType] = quantity;

        iconGrid?.PopulateGrid();
    }

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

    public bool HasMaterial(MaterialType materialType, int quantity)
    {
        if (materialType == null) return false;

        return materialQuantities.ContainsKey(materialType) && materialQuantities[materialType] >= quantity;
    }

    public void TryPickupItemFromShelf()
    {
        if (displayShelf != null && displayShelf.HasItem())
        {
            MaterialType material = displayShelf.GetItem();
            if (material == null) return;

            AddMaterial(material, 1);
            displayShelf.ClearItem();
        }
    }

    public void SaveInventory()
    {
        // Convert inventory to SerializableInventory
        SerializableInventory serializableInventory = new SerializableInventory();

        foreach (var item in materialQuantities)
        {
            SerializableInventory.InventoryItem inventoryItem = new SerializableInventory.InventoryItem
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

        // Optionally save the player position
        if (player != null)
        {
            Vector3 pos = player.position;
            PlayerPrefs.SetFloat("PlayerPosX", pos.x);
            PlayerPrefs.SetFloat("PlayerPosY", pos.y);
            PlayerPrefs.SetFloat("PlayerPosZ", pos.z);
        }
    }

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
                MaterialType material = FindMaterialByName(item.materialName);
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

    private MaterialType FindMaterialByName(string name)
    {
        if (allMaterials == null || allMaterials.Count == 0)
        {
            Debug.LogError("AllMaterials list is empty or not assigned!");
            return null;
        }

        foreach (MaterialType material in allMaterials)
        {
            if (material.materialName == name)
            {
                return material;
            }
        }

        Debug.LogWarning($"Material with name '{name}' not found.");
        return null;
    }
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
}



[System.Serializable]
public class SerializableInventory
{
    [System.Serializable]
    public class InventoryItem
    {
        public string materialName;
        public int quantity;
    }

    public List<InventoryItem> items = new List<InventoryItem>();
}
