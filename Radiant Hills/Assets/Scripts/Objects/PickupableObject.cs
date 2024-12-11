using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    public MaterialType materialType; // The type of material this object represents
    public int materialYield = 1; // The amount of material this object yields
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
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
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Inventory or MaterialType is missing!");
        }
    }
}
