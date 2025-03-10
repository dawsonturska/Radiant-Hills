using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("The door the player will be teleported to")]
    public Transform targetDoor; // The door to teleport to when the player interacts with this door

    [Tooltip("The offset from the target door's position when teleporting")]
    public Vector2 teleportOffset; // The offset to adjust the player's position after teleportation

    [Tooltip("The lock that needs to be triggered before the door works")]
    public ProjectileLock lockToTrigger; // The lock to check before teleporting

    // Called when the player enters the door collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player (or other object) enters the door trigger area
        if (other.CompareTag("Player"))
        {
            // Check if the lock is triggered, if necessary
            if (lockToTrigger == null || !lockToTrigger.IsLocked()) // Corrected to use IsLocked() here
            {
                Teleport(other.gameObject); // Teleport the player if the lock is triggered or not needed
            }
            else
            {
                Debug.Log("The lock is still active. Can't teleport!");
            }
        }
    }

    // Handle teleportation of the player
    private void Teleport(GameObject player)
    {
        // If the target door is assigned, teleport the player to it
        if (targetDoor != null)
        {
            // Apply the teleport offset to the target door's position
            player.transform.position = targetDoor.position + (Vector3)teleportOffset;

            // Optionally, you can reset the player's velocity here if you're using a Rigidbody2D
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Reset any movement or velocity after teleportation
            }
        }
        else
        {
            Debug.LogWarning("Target door not assigned on " + gameObject.name);
        }
    }
}
