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
    public DisplayShelf displayShelf;
    public int maxSlots = 20;

    private Queue<GameObject> iconPool = new Queue<GameObject>();
    private Queue<GameObject> emptySlotPool = new Queue<GameObject>();
    public bool IsGridPopulated { get; private set; } = false;

    void Start()
    {
        inventory ??= FindObjectOfType<Inventory>();
        if (SceneManager.GetActiveScene().name == "Shop")
        {
            displayShelf ??= FindObjectOfType<DisplayShelf>();
        }

        PopulateGrid();  // Initially populate the grid
    }

    public void UpdateUI()
    {
        Debug.Log("Updating UI...");
        PopulateGrid();  // Call PopulateGrid to refresh the grid
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
                    if (material.icon != null)
                    {
                        iconImage.sprite = material.icon;
                    }
                    else
                    {
                        Debug.LogWarning($"Material icon is missing for {material.materialName}");
                    }
                }

                // Remove any previously added listeners to avoid duplicates
                iconButton.onClick.RemoveAllListeners();

                // Capture the material for the click event
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

        IsGridPopulated = true;  // Mark the grid as populated
    }

    private void ClearGrid()
    {
        foreach (Transform child in gridContainer)
        {
            child.gameObject.SetActive(false);  // Deactivate instead of destroying to reuse later
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

        // Check if the player is in range of the display shelf and update the shelf
        if (displayShelf != null && displayShelf.IsPlayerInRange())
        {
            displayShelf.SetItem(material);  // Update the display shelf with the clicked material

            // Remove the item from inventory
            if (inventory.materialQuantities.ContainsKey(material))
            {
                inventory.materialQuantities[material]--;  // Decrease the quantity
                if (inventory.materialQuantities[material] <= 0)
                {
                    inventory.materialQuantities.Remove(material);  // If quantity reaches 0, remove the item
                }
            }

            // Update the UI after removing the item from the inventory
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("Display shelf is not assigned or player is not in range.");
        }
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
