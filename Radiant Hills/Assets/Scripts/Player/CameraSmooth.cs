using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform player; // Player transform
    public float smoothSpeed = 0.125f; // Smoothing factor
    public Vector3 offset; // Offset from the player

    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
