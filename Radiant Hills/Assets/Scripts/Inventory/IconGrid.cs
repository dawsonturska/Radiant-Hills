using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IconGrid : MonoBehaviour
{
    public GameObject itemIconPrefab;
    public GameObject emptySlotPrefab;
    public Transform gridContainer;
    public Inventory inventory;
    private DisplayShelf activeShelf;
    public int maxSlots = 20;

    private Queue<GameObject> iconPool = new Queue<GameObject>();
    private Queue<GameObject> emptySlotPool = new Queue<GameObject>();
    public bool IsGridPopulated { get; private set; } = false;

    void Start()
    {
        inventory ??= FindObjectOfType<Inventory>();

        if (SceneManager.GetActiveScene().name == "Shop")
        {
            // Automatically assign active shelf if player is in range of any shelf
            SetActiveShelfBasedOnPlayerRange();
        }

        PopulateGrid();  // Initially populate the grid
    }

    void Update()
    {
        // Recheck player range every frame to ensure we always have the correct shelf
        SetActiveShelfBasedOnPlayerRange();
    }

    private void SetActiveShelfBasedOnPlayerRange()
    {
        // Find the nearest shelf that the player is in range of
        var shelves = FindObjectsOfType<DisplayShelf>();  // Get all shelves in the scene
        DisplayShelf closestShelf = null;

        foreach (var shelf in shelves)
        {
            if (shelf.IsPlayerInRange()) // Check if player is in range of the shelf
            {
                // If a shelf is in range, check if it is closer or if no shelf is set yet
                if (closestShelf == null || Vector3.Distance(shelf.transform.position, transform.position) < Vector3.Distance(closestShelf.transform.position, transform.position))
                {
                    closestShelf = shelf;
                }
            }
        }

        if (closestShelf != null)
        {
            activeShelf = closestShelf;
            Debug.Log($"Active shelf set to Shelf {activeShelf.shelfID}");
        }
        else
        {
            activeShelf = null;
        }
    }

    public void UpdateUI()
    {
        Debug.Log("Updating UI...");
        PopulateGrid();  // Refresh the grid
    }

    public void PopulateGrid()
    {
        Debug.Log("Populating grid with " + inventory.materialQuantities.Count + " materials");

        ClearGrid();  // Clear existing grid content

        int slotIndex = 0;
        foreach (var entry in inventory.materialQuantities)
        {
            MaterialType material = entry.Key;
            int quantity = entry.Value;

            GameObject icon = GetIconFromPool();
            TextMeshProUGUI textMeshProComponent = icon.GetComponentInChildren<TextMeshProUGUI>();

            // Set the quantity text
            if (textMeshProComponent != null)
            {
                textMeshProComponent.text = quantity.ToString();
            }

            Button iconButton = icon.GetComponent<Button>();
            if (iconButton != null)
            {
                Image iconImage = icon.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = material.icon != null ? material.icon : null;
                }

                iconButton.onClick.RemoveAllListeners();
                iconButton.onClick.AddListener(() => OnItemClicked(material));
            }

            slotIndex++;

            if (slotIndex >= maxSlots)
            {
                break;
            }
        }

        // Fill remaining slots with empty slots if needed
        while (slotIndex < maxSlots)
        {
            GameObject emptySlot = GetEmptySlotFromPool();
            emptySlot.SetActive(true);
            slotIndex++;
        }

        IsGridPopulated = true;
    }

    private void ClearGrid()
    {
        foreach (Transform child in gridContainer)
        {
            child.gameObject.SetActive(false);
        }

        IsGridPopulated = false;
    }

    private GameObject GetIconFromPool()
    {
        if (iconPool.Count > 0)
        {
            GameObject icon = iconPool.Dequeue();
            icon.SetActive(true);
            return icon;
        }
        else
        {
            return Instantiate(itemIconPrefab, gridContainer);
        }
    }

    private GameObject GetEmptySlotFromPool()
    {
        if (emptySlotPool.Count > 0)
        {
            GameObject emptySlot = emptySlotPool.Dequeue();
            emptySlot.SetActive(true);
            return emptySlot;
        }
        else
        {
            return Instantiate(emptySlotPrefab, gridContainer);
        }
    }

    private void OnItemClicked(MaterialType material)
    {
        Debug.Log($"Item clicked: {material.materialName}");

        if (activeShelf == null)
        {
            Debug.LogWarning("No active shelf assigned! Cannot store item.");
            return;
        }

        if (!activeShelf.IsPlayerInRange())
        {
            Debug.LogWarning($"Shelf {activeShelf.shelfID} is not in range. Cannot store item.");
            return;
        }

        activeShelf.StoreItemInShelf(material);

        // Remove the item from inventory safely
        if (inventory.materialQuantities.TryGetValue(material, out int quantity))
        {
            if (quantity > 1)
            {
                inventory.materialQuantities[material]--;
            }
            else
            {
                inventory.materialQuantities.Remove(material);
            }
        }

        UpdateUI();
    }

    public void ReturnToPool(GameObject icon)
    {
        icon.SetActive(false);
        iconPool.Enqueue(icon);
    }

    public void ReturnEmptySlotToPool(GameObject emptySlot)
    {
        emptySlot.SetActive(false);
        emptySlotPool.Enqueue(emptySlot);
    }
}
