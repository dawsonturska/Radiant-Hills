using UnityEngine;

/// <summary>
/// Zone of Interaction for Register
/// </summary>
public class RegisterTrigger : MonoBehaviour
{
    // Reference to the parent Register script
    private Register parentRegister;

    private void Awake()
    {
        parentRegister = GetComponentInParent<Register>();
        if (parentRegister == null)
        {
            Debug.LogError("No Register script found on parent.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerHandler = collision.GetComponent<PlayerInputHandler>();
            if (playerHandler != null)
            {
                // parentRegister is Interactable, so handle that
                playerHandler.SetCurrentInteractable(parentRegister);
                parentRegister.SetPlayerInTrigger(true);
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
                // parentRegister is Interactable, so handle that
                playerHandler.ClearInteractable(parentRegister);
                parentRegister.SetPlayerInTrigger(false);
            }
        }
    }
}