using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayShelf : MonoBehaviour
{
    [Header("Shelf Display Components")]
    public SpriteRenderer itemDisplaySpriteRenderer;
    public Sprite emptySprite;

    [Header("Inventory & UI")]
    public GameObject inventoryPanel;

    private MaterialType currentMaterial;
    private bool isPlayerInRange = false;

    private Inventory inventory;
    private IconGrid iconGrid;
    private BoxCollider2D interactionCollider;

    [Header("Unique Shelf Identification")]
    public int shelfID;
    private static HashSet<int> usedIDs = new HashSet<int>();
    private static Dictionary<int, MaterialType> shelfStorage = new Dictionary<int, MaterialType>();

    void Start()
    {
        AssignUniqueID();
        InitializeComponents();
        LoadStoredItem();
    }

    private void AssignUniqueID()
    {
        if (shelfID == 0)
        {
            do
            {
                shelfID = Random.Range(1000, 9999);
            } while (usedIDs.Contains(shelfID));

            usedIDs.Add(shelfID);
        }

        Debug.Log($"Shelf {shelfID} initialized.");
    }

    private void InitializeComponents()
    {
        inventory = FindObjectOfType<Inventory>();
        iconGrid = FindObjectOfType<IconGrid>();

        if (itemDisplaySpriteRenderer == null)
        {
            itemDisplaySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (itemDisplaySpriteRenderer == null)
            {
                Debug.LogError($"Shelf {shelfID}: No SpriteRenderer found!");
                return;
            }
        }

        // Ensure a trigger collider exists and set up properly
        interactionCollider = GetComponent<BoxCollider2D>();
        if (interactionCollider == null)
        {
            interactionCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        interactionCollider.isTrigger = true;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Shelf {shelfID}: E pressed. Player in range? {isPlayerInRange}");

            if (HasItem())
            {
                PickupItem();
            }
            else
            {
                ToggleInventory();
            }
        }

        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isOpen = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isOpen);
            Debug.Log($"Shelf {shelfID}: Inventory {(isOpen ? "Closed" : "Opened")}");
        }
    }

    private void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log($"Shelf {shelfID}: Player entered range.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseInventory();
            Debug.Log($"Shelf {shelfID}: Player exited range.");
        }
    }

    public void StoreItemInShelf(MaterialType material)
    {
        // Ensure that we're checking for the correct shelf's range
        if (!isPlayerInRange)
        {
            Debug.LogWarning($"Shelf {shelfID}: Not in range, cannot store {material.name}.");
            return;
        }

        if (material == null)
        {
            Debug.LogWarning($"Shelf {shelfID}: Attempted to store null material.");
            return;
        }

        shelfStorage[shelfID] = material;
        currentMaterial = material;
        SetItemIcon(material);
        inventory.RemoveMaterial(material, 1);

        Debug.Log($"Shelf {shelfID}: Stored {material.name}.");
    }

    private void SetItemIcon(MaterialType material)
    {
        if (itemDisplaySpriteRenderer == null)
        {
            Debug.LogWarning($"Shelf {shelfID}: SpriteRenderer not assigned.");
            return;
        }

        itemDisplaySpriteRenderer.sprite = material.icon != null ? material.icon : emptySprite;
    }

    public void ClearItem()
    {
        if (shelfStorage.ContainsKey(shelfID))
        {
            shelfStorage.Remove(shelfID);
        }

        currentMaterial = null;
        itemDisplaySpriteRenderer.sprite = emptySprite;
        Debug.Log($"Shelf {shelfID}: Cleared.");
    }

    public bool HasItem()
    {
        return shelfStorage.ContainsKey(shelfID) && shelfStorage[shelfID] != null;
    }

    public bool IsPlayerInRange()
    {
        Debug.Log($"Shelf {shelfID}: Checking range. isPlayerInRange = {isPlayerInRange}");
        return isPlayerInRange;
    }

    public void OnItemClicked(MaterialType material)
    {
        if (material != null)
        {
            StoreItemInShelf(material);
            CloseInventory();
        }
    }

    public MaterialType GetItem()
    {
        return shelfStorage.ContainsKey(shelfID) ? shelfStorage[shelfID] : null;
    }

    private void PickupItem()
    {
        if (HasItem())
        {
            MaterialType pickedMaterial = shelfStorage[shelfID];

            inventory.AddMaterial(pickedMaterial, 1);
            Debug.Log($"Shelf {shelfID}: Picked up {pickedMaterial.name}.");

            ClearItem();
        }
    }

    private void LoadStoredItem()
    {
        if (shelfStorage.ContainsKey(shelfID))
        {
            currentMaterial = shelfStorage[shelfID];
            SetItemIcon(currentMaterial);
        }
    }
}
