using UnityEngine;

public class SortingOrderAdjuster : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject player; // Reference to the player

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null)
        {
            //Debug.LogError("Player reference is missing!");
            return;
        }

        // Compare player's position to object's position
        if (player.transform.position.y > transform.position.y)
        {
            // Player is above (higher Y position), set higher sorting order
            spriteRenderer.sortingOrder = 2; // Sorting Order for when player is above
        }
        else
        {
            // Player is below or at the same Y position, set lower sorting order
            spriteRenderer.sortingOrder = 0; // Sorting Order for when player is below
        }
    }
}