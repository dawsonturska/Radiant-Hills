using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SortingOrderAdjuster : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject player;
    public List<GameObject> customers = new List<GameObject>();

    public float yOffset = 0f; // Y Offset adjustable in Inspector

    private Vector3 lastPlayerPosition;
    private List<Vector3> lastCustomerPositions = new List<Vector3>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load event
        UpdateAllReferences();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when destroyed
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SortingOrderAdjuster: Scene '{scene.name}' loaded. Updating references.");
        UpdateAllReferences();
    }

    public void UpdateAllReferences()
    {
        UpdatePlayerReference();
        UpdateCustomerList();
    }

    public void UpdatePlayerReference()
    {
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("SortingOrderAdjuster: Player reference is still null! Make sure a Player is tagged correctly.");
        }
        else
        {
            Debug.Log("SortingOrderAdjuster: Player reference updated.");
        }
    }

    public void UpdateCustomerList()
    {
        customers.Clear();
        customers.AddRange(GameObject.FindGameObjectsWithTag("Customer"));
        lastCustomerPositions.Clear();

        foreach (var customer in customers)
        {
            if (customer != null)
            {
                lastCustomerPositions.Add(customer.transform.position);
            }
        }
    }

    void Update()
    {
        if (player != null && Vector3.Distance(player.transform.position, lastPlayerPosition) > 0.1f)
        {
            UpdateSortingOrder(player.transform.position);
            lastPlayerPosition = player.transform.position;
        }

        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i] != null && Vector3.Distance(customers[i].transform.position, lastCustomerPositions[i]) > 0.1f)
            {
                UpdateSortingOrder(customers[i].transform.position);
                lastCustomerPositions[i] = customers[i].transform.position;
            }
        }
    }

    public void UpdateSortingOrder(Vector3 characterPosition)
    {
        float adjustedPivotY = transform.position.y + yOffset; // Apply offset to pivot
        spriteRenderer.sortingOrder = characterPosition.y > adjustedPivotY ? 2 : 0;
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        UpdateAllReferences(); // Reinitialize everything when player is set
    }
}
