using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayShelf : MonoBehaviour
{
    [Header("Shelf Display Components")]
    public SpriteRenderer itemDisplaySpriteRenderer;
    public Sprite emptySprite;

    [Header("IconGrid (Assign in Inspector or Automatically)")]
    public IconGrid iconGrid;

    private MaterialType currentMaterial;
    private bool isPlayerInRange = false;
    private Inventory inventory;
    private BoxCollider2D interactionCollider;

    [Header("Unique Shelf Identification")]
    public int shelfID;
    private static HashSet<int> usedIDs = new HashSet<int>();
    private static Dictionary<int, MaterialType> shelfStorage = new Dictionary<int, MaterialType>();

    void Start()
    {
        AssignUniqueID();
        InitializeComponents();
        InitializeReferences();
        LoadStoredItem();
    }

    public void InitializeReferences()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory reference is missing in scene.");
        }

        // Ensure IconGrid is assigned dynamically if not set in Inspector
        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>();
            if (iconGrid == null)
            {
                Debug.LogError("IconGrid reference is missing! Ensure it is in the scene.");
            }
        }
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
        if (itemDisplaySpriteRenderer == null)
        {
            itemDisplaySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (itemDisplaySpriteRenderer == null)
            {
                Debug.LogError($"Shelf {shelfID}: No SpriteRenderer found!");
                return;
            }
        }

        interactionCollider = GetComponent<BoxCollider2D>();
        if (interactionCollider == null)
        {
            interactionCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        interactionCollider.isTrigger = true;
    }

    void Update()
    {
        if (itemDisplaySpriteRenderer == null || iconGrid == null)
        {
            return;
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (HasItem())
            {
                PickupItem();
            }
            else
            {
                ToggleInventory();
            }
        }

        if (iconGrid.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    private void ToggleInventory()
    {
        if (iconGrid != null)
        {
            bool isOpen = iconGrid.gameObject.activeSelf;
            iconGrid.gameObject.SetActive(!isOpen);
            Debug.Log($"Shelf {shelfID}: Inventory {(isOpen ? "Closed" : "Opened")}");
        }
    }

    private void CloseInventory()
    {
        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseInventory();
        }
    }

    public void StoreItemInShelf(MaterialType material)
    {
        if (!isPlayerInRange || material == null || !inventory.HasMaterial(material, 1))
        {
            return;
        }

        shelfStorage[shelfID] = material;
        currentMaterial = material;
        SetItemIcon(material);
        inventory.RemoveMaterial(material, 1);
    }

    private void SetItemIcon(MaterialType material)
    {
        itemDisplaySpriteRenderer.sprite = material.icon != null ? material.icon : emptySprite;
    }

    public void ClearItem()
    {
        shelfStorage.Remove(shelfID);
        currentMaterial = null;
        itemDisplaySpriteRenderer.sprite = emptySprite;
    }

    public bool HasItem()
    {
        return shelfStorage.ContainsKey(shelfID) && shelfStorage[shelfID] != null;
    }

    public void OnItemClicked(MaterialType material)
    {
        StoreItemInShelf(material);
        CloseInventory();
    }

    public MaterialType GetItem()
    {
        return shelfStorage.ContainsKey(shelfID) ? shelfStorage[shelfID] : null;
    }

    private void PickupItem()
    {
        if (HasItem())
        {
            inventory.AddMaterial(shelfStorage[shelfID], 1);
            ClearItem();
        }
    }

    public void LoadStoredItem()
    {
        if (shelfStorage.ContainsKey(shelfID))
        {
            currentMaterial = shelfStorage[shelfID];
            SetItemIcon(currentMaterial);
        }
        else
        {
            itemDisplaySpriteRenderer.sprite = emptySprite;
        }
    }

    public void SetIconGrid(IconGrid newIconGrid)
    {
        iconGrid = newIconGrid;
    }

    // Public property to expose 'isPlayerInRange'
    public bool IsPlayerInRange => isPlayerInRange;
}
