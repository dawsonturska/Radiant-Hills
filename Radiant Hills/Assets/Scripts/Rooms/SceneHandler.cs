using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;

    public PickupableObject[] pickupableObjects; // Reference to all pickupable objects in the scene
    public BreakableObject[] breakableObjects;   // Reference to all breakable objects
    public FishingManager fishingManager;         // Reference to the FishingManager
    public FishingZone[] fishingZones;            // Reference to all fishing zones
    public Inventory playerInventory;             // Reference to the player's inventory

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePickupableObject(PickupableObject obj)
    {
        // Example: Update material yield or other properties of the object
        obj.materialYield += 1; // Increase material yield as an example
    }

    public void UpdateBreakableObject(BreakableObject obj)
    {
        // Example: Update health or other properties of the breakable object
        obj.health -= 5; // Reduce health for example
    }

    public void StartFishing()
    {
        if (fishingManager != null && !fishingManager.IsFishing)
        {
            fishingManager.StartFishing(); // Start fishing
        }
    }

    public void StopFishing()
    {
        if (fishingManager != null && fishingManager.IsFishing)
        {
            fishingManager.StopFishing(); // Stop fishing
        }
    }

    public void UpdateInventory(MaterialType material, int quantity)
    {
        if (playerInventory != null)
        {
            playerInventory.AddMaterial(material, quantity); // Update inventory with new materials
        }
    }
}
