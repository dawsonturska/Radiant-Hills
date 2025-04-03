using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed = 2f; // Speed of the cloud movement
    private float startX; // Start position from spawner
    private float endX; // End position from spawner
    private float yPosition; // Y position from spawner
    private int direction; // Movement direction (1 for right, -1 for left)

    private CloudSpawner spawner;

    void Start()
    {
        // Find the spawner in the scene
        spawner = FindObjectOfType<CloudSpawner>();

        if (spawner != null)
        {
            // Set positions based on spawner
            startX = spawner.startX;
            endX = spawner.endX;

            // Instead of using the spawner's Y position, pick the Y position assigned during spawn
            yPosition = transform.position.y;

            // Randomly decide direction (1 = right, -1 = left)
            direction = Random.Range(0, 2) == 0 ? 1 : -1;

            // Set initial position based on direction
            transform.position = new Vector3(direction == 1 ? startX : endX, yPosition, 0);
        }
        else
        {
            Debug.LogError("CloudMover: No CloudSpawner found in scene!");
        }
    }

    void Update()
    {
        // Move cloud in the chosen direction
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        // Reset when reaching the end based on movement direction
        if (direction == 1 && transform.position.x >= endX)
        {
            transform.position = new Vector3(startX, yPosition, 0);
        }
        else if (direction == -1 && transform.position.x <= startX)
        {
            transform.position = new Vector3(endX, yPosition, 0);
        }
    }
}
