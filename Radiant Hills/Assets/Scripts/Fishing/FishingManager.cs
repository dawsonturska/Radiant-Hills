using UnityEngine;
using System.Collections;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    private bool isFishing = false;
    private bool isFishCaught = false;
    private bool isWindowOpen = false;
    private float fishingTimeMin = 3f;
    private float fishingTimeMax = 5f;
    private float catchWindowTime = 3f;
    public GameObject exclamationPrefab;
    public GameObject player;
    public FishingZone fishingZone;

    private Coroutine fishingCoroutine;

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
            fishingCoroutine = StartCoroutine(FishingCycle());
            Debug.Log("Fishing started.");
        }
    }

    public void StopFishing()
    {
        if (isFishing)
        {
            isFishing = false;
            isWindowOpen = false;

            if (fishingCoroutine != null)
            {
                StopCoroutine(fishingCoroutine);
                fishingCoroutine = null;
            }

            HideExclamationMark();
            Debug.Log("Fishing stopped.");
        }
    }

    private IEnumerator FishingCycle()
    {
        // Randomly select a fishing time
        float fishingTime = Random.Range(fishingTimeMin, fishingTimeMax);
        yield return new WaitForSeconds(fishingTime);

        if (!isFishing) yield break;

        ShowExclamationMark();
        isWindowOpen = true;

        // Start the catch window timer
        float catchWindowTimer = 0f;
        while (catchWindowTimer < catchWindowTime)
        {
            if (!isFishing)
            {
                HideExclamationMark();
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.E)) // Check for key press
            {
                CatchFish();
                yield break;
            }

            catchWindowTimer += Time.deltaTime;
            yield return null;
        }

        // If the player didn't catch the fish, hide the exclamation mark and reset
        HideExclamationMark();
        isWindowOpen = false;
        isFishing = false;

        Debug.Log("Fishing failed. Trying again.");
        // Automatically restart fishing if the fish wasn't caught
        StartFishing();
    }

    private void ShowExclamationMark()
    {
        if (exclamationPrefab != null && player != null)
        {
            GameObject exclamation = Instantiate(exclamationPrefab, player.transform.position + Vector3.up, Quaternion.identity);
            exclamation.transform.SetParent(player.transform);
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
        AddRandomMaterialToPlayer();
        HideExclamationMark();  // Ensure the exclamation mark disappears
        RestartFishingCycle();  // Restart the fishing cycle after catching the fish
    }

    private void AddRandomMaterialToPlayer()
    {
        if (fishingZone != null && fishingZone.materialTypes.Count > 0)
        {
            // Select a random material
            MaterialType randomMaterial = fishingZone.materialTypes[Random.Range(0, fishingZone.materialTypes.Count)];

            // Add exactly 1 of that material
            int quantityToAdd = 1;

            Debug.Log($"Player received: {quantityToAdd} x {randomMaterial.name}");

            if (player != null)
            {
                // Get the player's inventory
                Inventory playerInventory = player.GetComponent<Inventory>();
                if (playerInventory != null)
                {
                    // Add the material to the player's inventory
                    playerInventory.AddMaterial(randomMaterial, quantityToAdd);
                }
                else
                {
                    Debug.LogError("Player does not have an Inventory component.");
                }
            }
        }
    }

    // New function to handle restarting the fishing cycle
    private void RestartFishingCycle()
    {
        // Stop the current fishing cycle
        StopFishing();

        // Start a new fishing cycle immediately
        StartFishing();
    }
}
