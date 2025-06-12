using UnityEngine;

public class PickupableObject : MonoBehaviour, IInteractable
{
    public MaterialType materialType;
    public int materialYield = 1;
    private bool isInRange = false;

    [Header("Audio")]
    public AudioClip pickupSound; // Sound for picking up the item
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource if not found
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;

            // show interaction indicator
            IndicatorManager.Instance.ShowIndicator("Interact", this.transform);

            // set as current interactable
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                playerHandler.SetCurrentInteractable(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;

            // hide interaction indicator
            IndicatorManager.Instance.HideIndicator("Interact", this.transform);

            // clear interactable
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                playerHandler.ClearInteractable(this);
            }
        }
    }

    private void PickUp(GameObject player)
    {
        if (isInRange)
        {
            Inventory inventory = player.GetComponent<Inventory>();

            if (inventory != null && materialType != null)
            {
                inventory.AddMaterial(materialType, materialYield);
                Debug.Log($"Picked up {materialYield} x {materialType.materialName}");

                // Play the pickup sound
                PlayPickupSound();

                Destroy(gameObject);
            }
        }
    }

    private void PlayPickupSound()
    {
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound); // Play the pickup sound
        }
    }

    /// <summary>
    /// Handler for "Interact" actions
    /// </summary>
    public void Interact(PlayerInputHandler handler)
    {
        PickUp(handler.gameObject);
    }
}