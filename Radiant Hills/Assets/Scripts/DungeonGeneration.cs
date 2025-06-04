using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    public enum {
        EMPTY_TILE, // No tile placed here
        OPEN_TILE, // Empty tile with a filled adjecent tile
    }
    
    public int dungeon_bounds = 10;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceRoom(x, y)
    {

    }

    void Generate(int max_rooms = 10, int seed = 0)
    {
        Random.seed = seed;

        int used_rooms = 0;
        // Initialize dungeon grid
        int[,] map = new int[dungeon_bounds, dungeon_bounds];
        // Set origin point
        map[dungeon_bounds / 2][dungeon_bounds - 1] = 1;

        // Repeat until enough rooms are placed
        while (used_rooms < max_rooms)
        {
            for (int x = 0; x < dungeon_bounds; x++)
            for (int y = 0; y < dungeon_bounds; y++)
                if (map[x, y] == OPEN_TILE)
                {
                    if (Random.value > 0.5)
                    {
                        
                    }
                }
        }
    }
}
