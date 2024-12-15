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
        float fishingTime = Random.Range(fishingTimeMin, fishingTimeMax);
        yield return new WaitForSeconds(fishingTime);

        if (!isFishing) yield break;

        ShowExclamationMark();
        isWindowOpen = true;

        float catchWindowTimer = 0f;
        while (catchWindowTimer < catchWindowTime)
        {
            if (!isFishing)
            {
                HideExclamationMark();
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CatchFish();
                yield break;
            }

            catchWindowTimer += Time.deltaTime;
            yield return null;
        }

        HideExclamationMark();
        isWindowOpen = false;
        isFishing = false;

        Debug.Log("Fishing failed. Trying again.");
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
    }

    private void AddRandomMaterialToPlayer()
    {
        if (fishingZone != null && fishingZone.materialTypes.Count > 0)
        {
            MaterialType randomMaterial = fishingZone.materialTypes[Random.Range(0, fishingZone.materialTypes.Count)];
            int randomQuantity = Random.Range(1, 4);

            Debug.Log($"Player received: {randomQuantity} x {randomMaterial.name}");

            if (player != null)
            {
                Inventory playerInventory = player.GetComponent<Inventory>();
                if (playerInventory != null)
                {
                    playerInventory.AddMaterial(randomMaterial, randomQuantity);
                }
                else
                {
                    Debug.LogError("Player does not have an Inventory component.");
                }
            }
        }
    }
}
