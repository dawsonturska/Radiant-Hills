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
            SetActiveShelfBasedOnPlayerRange();
        }

        Debug.Log("Populating grid on Start.");
        PopulateGrid();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Shop")
        {
            SetActiveShelfBasedOnPlayerRange();
        }
    }

    private void SetActiveShelfBasedOnPlayerRange()
    {
        var shelves = FindObjectsOfType<DisplayShelf>();
        DisplayShelf closestShelf = null;

        foreach (var shelf in shelves)
        {
            if (shelf.IsPlayerInRange)
            {
                if (closestShelf == null || Vector3.Distance(shelf.transform.position, transform.position) < Vector3.Distance(closestShelf.transform.position, transform.position))
                {
                    closestShelf = shelf;
                }
            }
        }

        if (closestShelf != null)
        {
            Debug.Log($"Active shelf set: {closestShelf.shelfID}");
        }
        else
        {
            Debug.LogWarning("No shelf in range.");
        }

        activeShelf = closestShelf;
    }

    public void UpdateUI()
    {
        Debug.Log("Updating UI.");
        PopulateGrid();
    }

    public void PopulateGrid()
    {
        Debug.Log("Populating grid...");
        ClearGrid();

        int slotIndex = 0;
        foreach (var entry in inventory.materialQuantities)
        {
            MaterialType material = entry.Key;
            int quantity = entry.Value;

            Debug.Log($"Adding material: {material.materialName} with quantity: {quantity}");

            GameObject icon = GetIconFromPool();
            icon.SetActive(true);

            // Set the text for the quantity
            TextMeshProUGUI textMeshProComponent = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshProComponent != null)
            {
                textMeshProComponent.text = quantity.ToString();
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found on item icon.");
            }

            // Set the image for the icon
            Image iconImage = icon.GetComponent<Image>();
            if (iconImage != null && material.icon != null)
            {
                iconImage.sprite = material.icon;
            }
            else
            {
                Debug.LogWarning($"Material {material.materialName} has no icon assigned.");
            }

            // Assign the click listener
            Button iconButton = icon.GetComponent<Button>();
            if (iconButton != null)
            {
                iconButton.onClick.RemoveAllListeners();
                iconButton.onClick.AddListener(() => OnItemClicked(material));
            }
            else
            {
                Debug.LogWarning("No Button component found on item icon.");
            }

            // Move to the next slot
            slotIndex++;
            if (slotIndex >= maxSlots) break;
        }

        // Fill remaining slots with empty slots only if the slot isn't already occupied
        while (slotIndex < maxSlots)
        {
            GameObject emptySlot = GetEmptySlotFromPool();
            emptySlot.SetActive(true);
            slotIndex++;
        }

        IsGridPopulated = true;
        Debug.Log("Grid population complete.");
    }

    private void ClearGrid()
    {
        Debug.Log("Clearing grid.");
        foreach (Transform child in gridContainer)
        {
            if (!child.gameObject.activeSelf) continue;  // Skip if the slot is already inactive (empty slots).

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
            Debug.Log("Reusing icon from pool.");
            return icon;
        }

        Debug.Log("Instantiating new icon.");
        return Instantiate(itemIconPrefab, gridContainer);
    }

    private GameObject GetEmptySlotFromPool()
    {
        if (emptySlotPool.Count > 0)
        {
            GameObject emptySlot = emptySlotPool.Dequeue();
            emptySlot.SetActive(true);
            return emptySlot;
        }

        Debug.Log("Instantiating new empty slot.");
        return Instantiate(emptySlotPrefab, gridContainer);
    }

    private void OnItemClicked(MaterialType material)
    {
        Debug.Log($"Item clicked: {material.materialName}");

        if (activeShelf == null)
        {
            Debug.LogWarning("No active shelf assigned! Cannot store item.");
            return;
        }

        if (!activeShelf.IsPlayerInRange)
        {
            Debug.LogWarning($"Shelf {activeShelf.shelfID} is not in range. Cannot store item.");
            return;
        }

        activeShelf.StoreItemInShelf(material);
        Debug.Log($"Stored {material.materialName} in shelf {activeShelf.shelfID}.");
        UpdateUI();  // Reflect inventory changes after storage
    }

    public void ReturnToPool(GameObject icon)
    {
        icon.SetActive(false);
        iconPool.Enqueue(icon);
        Debug.Log("Returning icon to pool.");
    }

    public void ReturnEmptySlotToPool(GameObject emptySlot)
    {
        emptySlot.SetActive(false);
        emptySlotPool.Enqueue(emptySlot);
        Debug.Log("Returning empty slot to pool.");
    }
}
