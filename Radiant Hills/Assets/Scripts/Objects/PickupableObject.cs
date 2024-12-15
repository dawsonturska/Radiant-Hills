using UnityEngine;


public class PickupableObject : MonoBehaviour
{
    public MaterialType materialType;
    public int materialYield = 1;
    public GameObject indicatorPrefab;
    private GameObject activeIndicator;
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
            inventory.AddMaterial(materialType, materialYield);
            Debug.Log($"Picked up {materialYield} x {materialType.materialName}");
            Destroy(gameObject);
        }
    }

    private void ShowIndicator()
    {
        if (indicatorPrefab != null && activeIndicator == null)
        {
            activeIndicator = Instantiate(indicatorPrefab, transform.position + Vector3.up, Quaternion.identity);
            activeIndicator.transform.SetParent(transform);
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
