using UnityEngine;
using UnityEngine.Events;

public class SceneWarpDialogue : MonoBehaviour, IInteractable
{
    [Tooltip("Event that will be called when \"Interact\" is pressed while the player is in range.")]
    public UnityEvent onInteract;
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                playerHandler.SetCurrentInteractable(this);
                isInRange = true;
                IndicatorManager.Instance.ShowIndicator("Interact", this.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                playerHandler.ClearInteractable(this);
                isInRange = false;
                IndicatorManager.Instance.HideIndicator("Interact", this.transform);
            }
        }
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            onInteract?.Invoke();
        }
    }

    /// <summary>
    /// Handler for "Interact" action
    /// </summary>
    public void Interact(PlayerInputHandler handler)
    {
        if (isInRange) onInteract?.Invoke();
    }
}
