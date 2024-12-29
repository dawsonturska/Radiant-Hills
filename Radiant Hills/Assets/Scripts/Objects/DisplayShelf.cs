using UnityEngine;
using UnityEngine.UI;

public class DisplayShelf : MonoBehaviour
{
    public Image itemDisplayImage; // Reference to the image UI component
    public Text itemNameText; // Reference to the name text UI component

    // Set the displayed item on the shelf
    public void SetItem(MaterialType material)
    {
        // Set the item's icon
        if (itemDisplayImage != null && material.icon != null)
        {
            itemDisplayImage.sprite = material.icon;
        }

        // Set the item's name
        if (itemNameText != null)
        {
            itemNameText.text = material.materialName;
        }
    }
}
