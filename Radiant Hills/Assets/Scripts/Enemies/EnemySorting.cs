using UnityEngine;

public class EnemySorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject player; // Reference to the player (set this in the Inspector)

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Automatically find the player if it's not set manually
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); // Assuming the player has the "Player" tag
            if (player == null)
            {
                Debug.LogError("Player object is missing. Make sure the player has the 'Player' tag.");
            }
        }
    }

    void Update()
    {
        UpdateSorting(); // Ensure sorting is updated every frame
    }

    public void UpdateSorting()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
            return;
        }

        // Compare the player's position to the enemy object's position
        if (player.transform.position.y > transform.position.y)
        {
            // Player is above (higher Y position), set higher sorting order for enemy
            spriteRenderer.sortingOrder = 2; // Sorting Order for when player is above
        }
        else
        {
            // Player is below or at the same Y position, set lower sorting order for enemy
            spriteRenderer.sortingOrder = 0; // Sorting Order for when player is below
        }
    }

    public void SetPlayer(GameObject newPlayer)
    {
        this.player = newPlayer;
    }
}
