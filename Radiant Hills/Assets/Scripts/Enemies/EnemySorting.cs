using UnityEngine;

public class EnemySorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GameObject player; // Reference to the player

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        FindPlayer(); // Find the player when the scene starts
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer(); // Try finding the player again if it's missing
        }
        UpdateSorting(); // Ensure sorting is updated every frame
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find player by tag
        if (player == null)
        {
            Debug.LogWarning("EnemySorting: Player not found. Ensure the player has the 'Player' tag.");
        }
    }

    public void UpdateSorting()
    {
        if (player == null)
        {
            return; // Skip sorting if the player is still missing
        }

        // Compare the player's position to the enemy object's position
        spriteRenderer.sortingOrder = player.transform.position.y > transform.position.y ? 2 : 0;
    }

    public void SetPlayer(GameObject newPlayer)
    {
        this.player = newPlayer;
    }
}
