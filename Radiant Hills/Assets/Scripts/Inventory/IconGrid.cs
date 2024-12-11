using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IconGrid : MonoBehaviour
{
    // These fields need to be public or serialized so they show up in the Inspector
    public GameObject buttonPrefab; // Prefab for each icon button
    public Transform gridPanel; // The parent object (GridPanel) where buttons will be added
    public List<Sprite> iconSprites; // List of icons to be displayed in the grid

    void Start()
    {
        PopulateGrid();
    }

    public void PopulateGrid()
    {
        // Clear existing buttons in the grid (optional)
        foreach (Transform child in gridPanel)
        {
            Destroy(child.gameObject);
        }

        // Loop through each icon in the iconSprites list and create a button
        foreach (Sprite icon in iconSprites)
        {
            // Instantiate a button from the prefab
            GameObject button = Instantiate(buttonPrefab, gridPanel);

            // Set the icon image
            button.GetComponentInChildren<Image>().sprite = icon;

            // Add a listener for the button click event
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => OnIconClick(icon));
        }
    }

    // Method to handle icon click
    void OnIconClick(Sprite clickedIcon)
    {
        Debug.Log("Clicked on icon: " + clickedIcon.name);
        // You can trigger actions based on the clicked icon here (e.g., show info, add to inventory, etc.)
    }
}
