using UnityEngine;

public class ProjectileLock : MonoBehaviour
{
    private Collider2D lockCollider; // Collider for the lock
    private bool isLocked = true; // State of the lock, initially locked

    void Start()
    {
        // Get the collider component attached to the lock object
        lockCollider = GetComponent<Collider2D>();

        if (lockCollider == null)
        {
            Debug.LogError("Collider2D not found on the lock object.");
        }
    }

    // Method to be called when the lock is hit by a projectile
    public void Unlock()
    {
        if (lockCollider != null && isLocked)
        {
            isLocked = false;
            lockCollider.enabled = false; // Disable the collider, allowing the player to walk through
            Debug.Log($"{gameObject.name} unlocked!");
        }
    }

    // Optionally, you can reset the lock after a certain time or based on game logic
    public void ResetLock()
    {
        if (lockCollider != null && !isLocked)
        {
            isLocked = true;
            lockCollider.enabled = true; // Enable the collider again
            Debug.Log($"{gameObject.name} locked again!");
        }
    }

    // Method to check if the lock is currently triggered/unlocked
    public bool IsLocked()
    {
        return isLocked;
    }
}
