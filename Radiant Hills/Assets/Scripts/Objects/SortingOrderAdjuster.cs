using UnityEngine;
using System.Collections.Generic;

public class SortingOrderAdjuster : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject player; // Reference to the player GameObject
    private List<GameObject> customers = new List<GameObject>(); // List to hold customers
    private Vector3 lastPlayerPosition; // To track player's last position
    private List<Vector3> lastCustomerPositions = new List<Vector3>(); // To track customers' last positions

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If player is not assigned, try to find it dynamically
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Assuming the player has the "Player" tag
            if (player == null)
            {
                Debug.LogWarning("Player reference missing on Awake.");
            }
            else
            {
                Debug.Log("Player found in Awake.");
                lastPlayerPosition = player.transform.position; // Initialize last position
            }
        }

        // Find all customers
        customers.AddRange(GameObject.FindGameObjectsWithTag("Customer"));
        foreach (var customer in customers)
        {
            lastCustomerPositions.Add(customer.transform.position);
        }
    }

    void Start()
    {
        // Check if player reference is still missing after Awake
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Try to find again in Start
            if (player == null)
            {
                Debug.LogWarning("Player reference missing on Start.");
            }
            else
            {
                Debug.Log("Player found in Start.");
                lastPlayerPosition = player.transform.position; // Initialize last position
            }
        }

        // Find customers in case they're dynamically spawned after Awake
        customers.AddRange(GameObject.FindGameObjectsWithTag("Customer"));
        foreach (var customer in customers)
        {
            lastCustomerPositions.Add(customer.transform.position);
        }
    }

    void OnEnable()
    {
        // Additional check when the object is enabled
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing in SortingOrderAdjuster on enable.");
        }
        else
        {
            Debug.Log("Player found on enable.");
        }

        // Re-check for customers when the object is enabled
        customers.Clear();
        customers.AddRange(GameObject.FindGameObjectsWithTag("Customer"));
        lastCustomerPositions.Clear();
        foreach (var customer in customers)
        {
            lastCustomerPositions.Add(customer.transform.position);
        }
    }

    void Update()
    {
        // Update sorting order for the player
        if (player != null && Vector3.Distance(player.transform.position, lastPlayerPosition) > 0.1f)
        {
            UpdateSortingOrder(player.transform.position, lastPlayerPosition);
            lastPlayerPosition = player.transform.position; // Update last position
        }

        // Update sorting order for each customer
        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i] != null && Vector3.Distance(customers[i].transform.position, lastCustomerPositions[i]) > 0.1f)
            {
                UpdateSortingOrder(customers[i].transform.position, lastCustomerPositions[i]);
                lastCustomerPositions[i] = customers[i].transform.position; // Update last customer position
            }
        }
    }

    public void UpdateSortingOrder(Vector3 characterPosition, Vector3 lastPosition)
    {
        if (characterPosition.y > transform.position.y)
        {
            spriteRenderer.sortingOrder = 2; // Sorting Order for when character is above
        }
        else
        {
            spriteRenderer.sortingOrder = 0; // Sorting Order for when character is below
        }
    }
}
