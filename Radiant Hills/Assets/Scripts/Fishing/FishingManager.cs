using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    public GameObject exclamationMarkPrefab;  // Reference to the exclamation mark prefab
    private GameObject exclamationMarkInstance;  // The instance of the exclamation mark above the player
    private bool isFishing = false;

    public float fishingWaitTimeMin = 2f;  // Minimum wait time before exclamation mark appears
    public float fishingWaitTimeMax = 5f;  // Maximum wait time before exclamation mark appears

    // List of possible drops (added multiple times to represent their "weight")
    public List<MaterialType> possibleDrops = new List<MaterialType>();

    // The player's inventory (use the Inventory script to manage)
    public Inventory playerInventory;

    public Transform playerTransform;  // Reference to the player's Transform (drag the player object here)

    void Awake()
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

    public void StartFishing()
    {
        if (!isFishing)
        {
            isFishing = true;
            StartCoroutine(FishingCoroutine());
        }
    }

    private IEnumerator FishingCoroutine()
    {
        // Wait for a random amount of time before showing the exclamation mark
        float waitTime = Random.Range(fishingWaitTimeMin, fishingWaitTimeMax);
        yield return new WaitForSeconds(waitTime);

        // Show the exclamation mark
        ShowExclamationMark();

        // Wait for the player to press 'E' to catch the fish
        bool isFishCaught = false;
        while (!isFishCaught)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isFishCaught = true;
                CatchFish();
            }
            yield return null;
        }
    }

    private void ShowExclamationMark()
    {
        // Instantiate and position the exclamation mark above the player, offset by 0.75 units on the Y-axis
        if (exclamationMarkPrefab != null && playerTransform != null)
        {
            Vector3 offsetPosition = playerTransform.position + new Vector3(0, 0.75f, 0); // Offset by 0.75 units on the Y-axis
            exclamationMarkInstance = Instantiate(exclamationMarkPrefab, offsetPosition, Quaternion.identity);
            exclamationMarkInstance.transform.SetParent(playerTransform);  // Set the exclamation mark's parent to the player (or world space)
        }
    }

    private void CatchFish()
    {
        // Hide the exclamation mark and perform the fish catching behavior
        if (exclamationMarkInstance != null)
        {
            Destroy(exclamationMarkInstance);
        }

        Debug.Log("Fish caught!");

        // Guarantee a drop and pick one randomly from the possible drops list
        AddRandomDropToInventory();

        isFishing = false;
    }

    private void AddRandomDropToInventory()
    {
        // Check if there are any possible drops
        if (possibleDrops.Count > 0)
        {
            // Randomly select an item from the possible drops
            int randomIndex = Random.Range(0, possibleDrops.Count);
            MaterialType selectedDrop = possibleDrops[randomIndex];

            // Add the selected item to the inventory
            playerInventory.AddMaterial(selectedDrop);
            Debug.Log($"{selectedDrop.materialName} added to inventory!");
        }
        else
        {
            Debug.Log("No drops available.");
        }
    }
}
