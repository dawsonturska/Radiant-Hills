using UnityEngine;

public class DisplayShelf : MonoBehaviour
{
    private SpriteRenderer itemDisplaySpriteRenderer; // Reference to the parent SpriteRenderer component
    private bool isPlayerInRange = false; // Tracks if the player is within range of the shelf
    private MaterialType currentMaterial; // Store the current material for the shelf

    void Awake()
    {
        // Attempt to get the SpriteRenderer component from the parent
        itemDisplaySpriteRenderer = GetComponentInParent<SpriteRenderer>();

        if (itemDisplaySpriteRenderer == null)
        {
            Debug.LogWarning("Parent does not have a SpriteRenderer component.");
        }
    }

    // Set the displayed item on the shelf
    public void SetItem(MaterialType material)
    {
        if (material == null)
        {
            Debug.LogWarning("Trying to set a null material.");
            return;
        }

        currentMaterial = material; // Store the material
        SetItemSprite(material); // Set the item's sprite
    }

    // Helper to set the material sprite
    private void SetItemSprite(MaterialType material)
    {
        if (itemDisplaySpriteRenderer == null)
        {
            Debug.LogWarning("Parent SpriteRenderer component is not assigned.");
            return;
        }

        if (material.icon != null)
        {
            itemDisplaySpriteRenderer.sprite = material.icon;
        }
        else
        {
            Debug.LogWarning($"Sprite is missing for material: {material.materialName}");
        }
    }

    // Detect when the player enters the interaction field (trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = true;
            Debug.Log("Player entered display shelf range.");
        }
    }

    // Detect when the player exits the interaction field (trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the interacting object is the player
        {
            isPlayerInRange = false;
            Debug.Log("Player exited display shelf range.");
        }
    }

    // Method to check if the player is in range to interact with the shelf
    public bool IsPlayerInRange()
    {
        return isPlayerInRange;
    }
}
