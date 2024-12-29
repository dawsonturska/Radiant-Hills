using UnityEngine;

public class DisplayShelf : MonoBehaviour
{
    public SpriteRenderer itemDisplaySpriteRenderer;
    private MaterialType currentMaterial;
    public Sprite emptySprite;
    private bool isPlayerInRange = false;
    private Transform playerTransform;
    public Inventory inventory; // This will now be assigned dynamically.
    public IconGrid iconGrid; // Reference to the IconGrid

    void Start()
    {
        playerTransform = Camera.main.transform;

        // Dynamically assign the Inventory component
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory not found in the scene.");
        }
    }

    public void SetIconGrid(IconGrid newIconGrid)
    {
        if (newIconGrid != null)
        {
            iconGrid = newIconGrid;
            Debug.Log("DisplayShelf IconGrid updated.");
        }
        else
        {
            Debug.LogWarning("Received null IconGrid reference for DisplayShelf.");
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
        }
    }

    public void SetItem(MaterialType material)
    {
        if (material == null)
        {
            Debug.LogWarning("Trying to set a null material.");
            return;
        }

        currentMaterial = material;
        SetItemIcon(material);
    }

    private void SetItemIcon(MaterialType material)
    {
        if (itemDisplaySpriteRenderer == null)
        {
            Debug.LogWarning("Item display sprite renderer is not assigned.");
            return;
        }

        if (material.icon != null)
        {
            itemDisplaySpriteRenderer.sprite = material.icon;
        }
        else
        {
            Debug.LogWarning($"Icon is missing for material: {material.materialName}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered display shelf range.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited display shelf range.");
        }
    }

    private void PickupItem()
    {
        if (currentMaterial != null)
        {
            inventory.AddMaterial(currentMaterial, 1); // Add item to the player's inventory
            ClearItem(); // Remove the item from the display shelf
        }
        else
        {
            Debug.Log("No item to pick up.");
        }
    }

    public bool IsPlayerInRange() => isPlayerInRange;

    public void ClearItem()
    {
        currentMaterial = null;
        itemDisplaySpriteRenderer.sprite = emptySprite; // Clear the shelf display
    }

    // Get the currently displayed item on the shelf
    public MaterialType GetItem()
    {
        return currentMaterial;
    }

    // Check if there's an item currently displayed on the shelf
    public bool HasItem()
    {
        return currentMaterial != null;
    }
}
