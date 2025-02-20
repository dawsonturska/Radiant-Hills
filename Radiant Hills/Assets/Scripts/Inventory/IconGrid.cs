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

        PopulateGrid();
    }

    void Update()
    {
        SetActiveShelfBasedOnPlayerRange();
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

        activeShelf = closestShelf;
    }

    public void UpdateUI()
    {
        PopulateGrid();
    }

    public void PopulateGrid()
    {
        ClearGrid();

        int slotIndex = 0;
        foreach (var entry in inventory.materialQuantities)
        {
            MaterialType material = entry.Key;
            int quantity = entry.Value;

            GameObject icon = GetIconFromPool();
            icon.SetActive(true);

            TextMeshProUGUI textMeshProComponent = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshProComponent != null)
            {
                textMeshProComponent.text = quantity.ToString();
            }

            Image iconImage = icon.GetComponent<Image>();
            if (iconImage != null && material.icon != null)
            {
                iconImage.sprite = material.icon;
            }

            Button iconButton = icon.GetComponent<Button>();
            if (iconButton != null)
            {
                iconButton.onClick.RemoveAllListeners();
                iconButton.onClick.AddListener(() => OnItemClicked(material));
            }

            slotIndex++;
            if (slotIndex >= maxSlots) break;
        }

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
        UpdateUI();  // Reflect inventory changes after storage
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
