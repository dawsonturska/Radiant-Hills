using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset; // Offset distance between the camera and the player
    public float followSpeed = 10f; // Speed at which the camera follows the player

    private Camera mainCamera;

    void Start()
    {
        // Ensure the player is assigned
        if (player == null)
        {
            Debug.LogError("Player reference is missing in CameraFollow.");
            return;
        }

        // Get the main camera if it's not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            // Set the camera's initial position relative to the player
            mainCamera.transform.SetParent(player);
            mainCamera.transform.localPosition = offset; // Apply the offset in local space
            mainCamera.transform.localRotation = Quaternion.identity; // Optional: Reset rotation
        }
        else
        {
            Debug.LogError("No main camera found!");
        }
    }

    void Update()
    {
        // Make sure the camera follows the player with a smooth transition
        if (mainCamera != null)
        {
            Vector3 targetPosition = player.position + offset; // Calculate target position
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    // Method to dynamically change the player target
    public void SetTarget(Transform newPlayer)
    {
        player = newPlayer;

        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(player); // Reattach camera to the new player
            mainCamera.transform.localPosition = offset; // Reset position relative to new player
        }
    }
}
