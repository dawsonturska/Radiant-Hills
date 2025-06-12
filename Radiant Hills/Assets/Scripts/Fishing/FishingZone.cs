using System.Collections.Generic;
using UnityEngine;

public class FishingZone : MonoBehaviour, IInteractable
{
    // Define a list of materials to be caught
    public List<MaterialType> materialTypes; // Define this list in the inspector for available items to catch

    private bool isPlayerInZone = false; // Track if the player is inside the zone


    /// PRIVATE METHODS ///

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the zone
        if (other.CompareTag("Player"))
        {
            var playerHandler = other.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                // hide interaction indicator
                IndicatorManager.Instance.ShowIndicator("Interact", this.transform);

                // set current interactable
                playerHandler.SetCurrentInteractable(this);

                isPlayerInZone = true;
                FishingManager.Instance.SetFishingZone(this);
                Debug.Log("Player entered the fishing zone.");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the zone
        if (other.CompareTag("Player"))
        {
            var playerHandler = other.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                // clear interactable
                playerHandler.ClearInteractable(this);
                isPlayerInZone = false;
                Debug.Log("Player exited the fishing zone.");

                // Stop fishing if the player is currently fishing
                if (FishingManager.Instance.IsFishing)
                {
                    FishingManager.Instance.StopFishing();
                    Debug.Log("Player stopped fishing because they exited the fishing zone.");
                }
                FishingManager.Instance.SetFishingZone(null);
            }
        }
    }

    /// PUBLIC METHODS ///

    /// <summary>
    /// Handler for "Interact" action
    /// </summary>
    public void Interact(PlayerInputHandler handler)
    {
        // Only allow interaction if the player is in the zone and not already fishing
        if (isPlayerInZone) {
            if (!FishingManager.Instance.IsFishing)
            {
                Debug.Log("Player pressed \"Interact\" to start fishing.");
                FishingManager.Instance.StartFishing();
                IndicatorManager.Instance.HideIndicator("Interact", this.transform);
            } else
            {
                Debug.Log("Player pressed \"Interact\" to try to catch fish.");
                FishingManager.Instance.TryCatchFish();
            }
            
        }
    }
}
