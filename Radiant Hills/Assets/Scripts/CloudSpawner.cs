using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab; // Assign a cloud prefab in the inspector
    public int cloudCount = 5; // Number of clouds
    public float minSpeed = 1f, maxSpeed = 3f; // Speed range
    public float minY = 2f, maxY = 5f; // Y position range for clouds
    public float startX = -12f, endX = 12f; // Screen bounds for spawning on the X axis
    public float ySpreadFactor = 5f; // Factor for increasing Y spread (can be adjusted)

    private List<GameObject> clouds = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            SpawnCloud();
        }
    }

    void SpawnCloud()
    {
        // Get the spawner's position
        Vector3 spawnPosition = transform.position;

        // Offset clouds relative to the spawner's Y position
        float spawnX = Random.Range(startX, endX);

        // Adjust the Y position to be offset from the spawner's position with a spread factor
        // This ensures the clouds are spread out from the spawner's position.
        float spawnY = spawnPosition.y + Random.Range(minY, maxY) + Random.Range(-ySpreadFactor, ySpreadFactor);

        // Instantiate the cloud at the calculated position
        GameObject newCloud = Instantiate(cloudPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);

        // Set the cloud's speed
        newCloud.AddComponent<CloudMover>().speed = Random.Range(minSpeed, maxSpeed);

        // Add to the list of clouds
        clouds.Add(newCloud);
    }
}
