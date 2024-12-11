using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // The player object
    public Vector3 offset; // The offset from the player
    public float smoothSpeed = 0.125f; // How smoothly the camera follows

    void LateUpdate()
    {
        // Desired position of the camera based on player position and offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly move from current position to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position
        transform.position = smoothedPosition;
    }
}
