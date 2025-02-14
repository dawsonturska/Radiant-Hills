using UnityEngine;
using System.Collections.Generic;

public class SortingOrderAdjuster : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject player;
    public List<GameObject> customers = new List<GameObject>();

    private Vector3 lastPlayerPosition;
    private List<Vector3> lastCustomerPositions = new List<Vector3>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateReferences();
    }

    public void UpdateReferences()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        UpdateCustomerList();
    }

    public void UpdateCustomerList()
    {
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
        spriteRenderer.sortingOrder = characterPosition.y > transform.position.y ? 2 : 0;
    }
}
