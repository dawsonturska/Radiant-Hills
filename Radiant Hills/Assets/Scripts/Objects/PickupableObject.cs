using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    public MaterialType materialType; // The type of material this object represents
    public int materialYield = 1; // The amount of material this object yields
    public GameObject indicatorPrefab; // The prefab to display above the object
    private GameObject activeIndicator; // Reference to the active indicator instance
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            ShowIndicator();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            HideIndicator();
        }
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Inventory inventory = player.GetComponent<Inventory>();

        if (inventory != null && materialType != null)
        {
            for (int i = 0; i < materialYield; i++) // Add the material multiple times based on yield
            {
                inventory.AddMaterial(materialType);
            }

            Debug.Log($"Picked up {materialYield} x {materialType.materialName}");
            Destroy(gameObject); // Destroy the pickup object
        }
        else
        {
            Debug.LogError("Inventory or MaterialType is missing!");
        }
    }

    private void ShowIndicator()
    {
        if (indicatorPrefab != null && activeIndicator == null) // Ensure no duplicate indicators
        {
            activeIndicator = Instantiate(indicatorPrefab, transform.position + Vector3.up, Quaternion.identity);
            activeIndicator.transform.SetParent(transform); // Make the indicator a child of the object
        }
    }

    private void HideIndicator()
    {
        if (activeIndicator != null)
        {
            Destroy(activeIndicator);
        }
    }
}
