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
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();  // Ensure Inventory reference is assigned
        }

        if (SceneManager.GetActiveScene().name == "Shop")
        {
            if (displayShelf == null)
            {
                displayShelf = FindObjectOfType<DisplayShelf>();
            }
        }

        PopulateGrid();  // Initially populate the grid
    }

    // This method will be called to update the grid UI, either to refresh or populate it again.
    public void UpdateUI()
    {
        Debug.Log("Updating UI...");
        PopulateGrid();  // Call PopulateGrid to refresh the grid
    }

    // Populate the grid with inventory items
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
                        Debug.LogWarning("Material icon is missing for " + material.materialName);
                    }
                }

                int capturedIndex = slotIndex;
                iconButton.onClick.AddListener(() => OnItemClicked(capturedIndex));
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

    // Clear all items from the grid
    private void ClearGrid()
    {
        foreach (Transform child in gridContainer)
        {
            child.gameObject.SetActive(false);
        }

        IsGridPopulated = false;
    }

    // Get an item icon from the pool or instantiate a new one
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

    // Get an empty slot from the pool or instantiate a new one
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

    // Handle item click events
    private void OnItemClicked(int index)
    {
        // Handle item click logic here
        Debug.Log("Item clicked at index: " + index);
    }
}
