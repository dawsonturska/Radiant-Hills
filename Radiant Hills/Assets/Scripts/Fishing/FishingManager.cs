using UnityEngine;
using System.Collections;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    private bool isFishing = false; // Track if the player is currently fishing
    private bool isFishCaught = false; // Track if the fish was caught
    private bool isWindowOpen = false; // Track if the catch window is open
    private float fishingTimeMin = 3f; // Minimum fishing time in seconds
    private float fishingTimeMax = 5f; // Maximum fishing time in seconds
    private float catchWindowTime = 3f; // Time window to catch the fish
    public GameObject exclamationPrefab; // The exclamation mark prefab
    public GameObject player; // Reference to the player
    public FishingZone fishingZone; // Reference to the fishing zone

    public bool IsFishing => isFishing;

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
            StartCoroutine(FishingCycle());
            Debug.Log("Fishing started.");
        }
    }

    private IEnumerator FishingCycle()
    {
        // Randomly select a fishing time between min and max
        float fishingTime = Random.Range(fishingTimeMin, fishingTimeMax);
        yield return new WaitForSeconds(fishingTime);

        // Show exclamation mark and allow player to catch the fish for 3 seconds
        ShowExclamationMark();
        isWindowOpen = true;

        float catchWindowTimer = 0f;
        while (catchWindowTimer < catchWindowTime)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Player pressed E to catch the fish
            {
                CatchFish();
                yield break;
            }

            catchWindowTimer += Time.deltaTime;
            yield return null;
        }

        // If the player didn't press E in time, hide the exclamation mark and restart
        HideExclamationMark();
        isWindowOpen = false;
        isFishing = false;
        Debug.Log("Fishing failed. Trying again.");
        StartFishing(); // Restart the fishing cycle
    }

    private void ShowExclamationMark()
    {
        if (exclamationPrefab != null && player != null)
        {
            GameObject exclamation = Instantiate(exclamationPrefab, player.transform.position + Vector3.up, Quaternion.identity);
            exclamation.transform.SetParent(player.transform); // Make the exclamation mark a child of the player
        }
    }

    private void HideExclamationMark()
    {
        foreach (Transform child in player.transform)
        {
            if (child.gameObject.CompareTag("ExclamationMark"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CatchFish()
    {
        isFishCaught = true;
        Debug.Log("Fish caught!");

        // Add random material to the player's inventory
        AddRandomMaterialToPlayer();

        // Reset the fishing cycle
        isFishing = false;
        isWindowOpen = false;
        HideExclamationMark(); // Hide the exclamation mark after catching the fish
        StartFishing(); // Restart the fishing cycle
    }

    private void AddRandomMaterialToPlayer()
    {
        if (fishingZone != null && fishingZone.materialTypes.Count > 0)
        {
            // Randomly select a material from the FishingZone's materialTypes list
            MaterialType randomMaterial = fishingZone.materialTypes[Random.Range(0, fishingZone.materialTypes.Count)];
            Debug.Log("Player received: " + randomMaterial.name);

            // Ensure the player has an inventory and add the random material
            if (player != null)
            {
                Inventory playerInventory = player.GetComponent<Inventory>(); // Assuming the player has an Inventory component
                if (playerInventory != null)
                {
                    playerInventory.AddMaterial(randomMaterial); // Add the selected material to the player's inventory
                }
                else
                {
                    Debug.LogError("Player does not have an Inventory component.");
                }
            }
        }
    }
}
