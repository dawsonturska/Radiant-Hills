using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controller for fishing. Only one instance needs to be
/// </summary>
public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    private bool isFishing = false;
    private bool isFishCaught = false;
    private bool isWindowOpen = false;
    private float fishingTimeMin = 3f;
    private float fishingTimeMax = 5f;
    private float catchWindowTime = 3f;

    public GameObject player;

    private FishingZone fishingZone;
    private Coroutine fishingCoroutine;

    public bool IsFishing => isFishing;


    /// PRIVATE METHODS ///

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when destroyed
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"FishingManager: Scene '{scene.name}' loaded. Updating references.");
        UpdateReferences();
    }

    private void UpdateReferences()
    {
        UpdatePlayerReference();
        UpdateFishingZoneReference();
    }

    private void UpdatePlayerReference()
    {
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("FishingManager: Player reference is still null! Make sure a Player is tagged correctly.");
        }
        else
        {
            Debug.Log("FishingManager: Player reference updated.");
        }
    }

    private void UpdateFishingZoneReference()
    {
        fishingZone = FindObjectOfType<FishingZone>();

        if (fishingZone == null)
        {
            Debug.LogError("FishingManager: No FishingZone found in the scene.");
        }
        else
        {
            Debug.Log("FishingManager: FishingZone reference updated.");
        }
    }

    private IEnumerator FishingCycle()
    {
        float fishingTime = Random.Range(fishingTimeMin, fishingTimeMax);
        yield return new WaitForSeconds(fishingTime);

        if (!isFishing) yield break;

        IndicatorManager.Instance.ShowIndicator("Exclamation", player.transform);
        isWindowOpen = true;

        float catchWindowTimer = 0f;
        while (catchWindowTimer < catchWindowTime)
        {
            if (!isFishing)
            {
                IndicatorManager.Instance.HideIndicator("Exclamation", player.transform);
                yield break;
            }

            if (!isWindowOpen) yield break;

            catchWindowTimer += Time.deltaTime;
            yield return null;
        }

        IndicatorManager.Instance.HideIndicator("Exclamation", player.transform);
        isWindowOpen = false;
        isFishing = false;

        Debug.Log("Fishing failed. Trying again.");
        StartFishing();
    }

    private void AddRandomMaterialToPlayer()
    {
        if (fishingZone != null && fishingZone.materialTypes.Count > 0)
        {
            MaterialType randomMaterial = fishingZone.materialTypes[Random.Range(0, fishingZone.materialTypes.Count)];
            int quantityToAdd = 1;

            Debug.Log($"Player received: {quantityToAdd} x {randomMaterial.name}");

            if (player != null)
            {
                Inventory playerInventory = player.GetComponent<Inventory>();
                if (playerInventory != null)
                {
                    playerInventory.AddMaterial(randomMaterial, quantityToAdd);
                }
                else
                {
                    Debug.LogError("Player does not have an Inventory component.");
                }
            }
        }
    }

    private void RestartFishingCycle()
    {
        StopFishing();
        StartFishing();
    }


    /// PUBLIC METHODS ///

    /// <summary>
    /// Initialize fishing cycle if not currently fishing
    /// </summary>
    public void StartFishing()
    {
        if (!isFishing)
        {
            isFishing = true;
            fishingCoroutine = StartCoroutine(FishingCycle());
            Debug.Log("Fishing started.");
        }
    }

    /// <summary>
    /// Stop fishing cycle if fishing
    /// </summary>
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

            IndicatorManager.Instance.HideIndicator("Exclamation", player.transform);
            Debug.Log("Fishing stopped.");
        }
    }

    /// <summary>
    /// Set current fishing zone that manager is handling
    /// </summary>
    public void SetFishingZone(FishingZone zone) { fishingZone = zone; }

    /// <summary>
    /// Catch fish if window is open. Called from FishingZone.
    /// </summary>
    public void TryCatchFish()
    {
        if (isWindowOpen)
        {
            isFishCaught = true;
            Debug.Log("Fish caught!");
            AddRandomMaterialToPlayer();
            IndicatorManager.Instance.HideIndicator("Exclamation", player.transform);
            RestartFishingCycle();
            isWindowOpen = false;
        }
    }
}