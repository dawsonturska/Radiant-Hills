using UnityEngine;
using UnityEngine.UI; // For Image and Text UI components

public class InventorySlot : MonoBehaviour
{
    public MaterialType materialType; // Material associated with this slot
    public int quantity; // Quantity of the material in this slot
    public Image itemIcon; // Image component to display the item sprite
    public Text quantityText; // Text component to display quantity overlay (if needed)

    // This method updates the slot based on the material type and quantity
    public void UpdateSlot()
    {
        if (materialType != null)
        {
            // Update the slot icon with the material's sprite
            itemIcon.sprite = materialType.icon; // Assuming MaterialType has an icon field

            // If the quantity is greater than 16, show the quantity overlay
            if (quantity > 16)
            {
                // Show a quantity text overlay
                quantityText.gameObject.SetActive(true);
                quantityText.text = quantity.ToString();
            }
            else
            {
                // Hide the quantity overlay if it's 16 or less
                quantityText.gameObject.SetActive(false);
            }
        }
    }
}
