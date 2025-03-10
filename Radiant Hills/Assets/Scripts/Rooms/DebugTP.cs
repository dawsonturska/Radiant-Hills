using UnityEngine;

public class TeleportToPoint : MonoBehaviour
{
    public Transform teleportLocation; // The location to teleport the player to

    void Update()
    {
        // Check for the * key press (Keypad Multiply key)
        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) // This should work for the * key
        {
            TeleportPlayer();
        }
    }

    // Teleport the player to the designated teleport location
    void TeleportPlayer()
    {
        if (teleportLocation != null)
        {
            // Assuming the player has a tag "Player" (you can change this based on your setup)
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = teleportLocation.position;
                Debug.Log("Player teleported to: " + teleportLocation.position);
            }
            else
            {
                Debug.LogError("Player not found!");
            }
        }
        else
        {
            Debug.LogError("Teleport location not assigned!");
        }
    }
}
