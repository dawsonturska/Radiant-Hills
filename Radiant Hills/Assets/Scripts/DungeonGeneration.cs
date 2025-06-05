using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    // Dungeon layout
    int[,] map;

    // Values used to fill tiles of "map"
    int
        // Unfilled tiles
        EMPTY = 0,
        // Unfilled tiles connected to filled tiles
        OPEN = -1,
        // Open tiles about to be filled
        QUEUED = -2,

        // The doors of a room are stored in 4 bits,
        // one for each direction.
        // For example, a room with exits leading upward
        // and to the right has a value of 9 (1 + 8).
        LEFT = 1,
        RIGHT = 2,
        UP = 4,
        DOWN = 8;

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

    void PlaceRoom(int x, int y)
    {

    }

    void Generate(int max_rooms = 10, int seed = 0)
    {
        Random.seed = seed;

        // Initialize dungeon grid
        map = new int[dungeon_bounds, dungeon_bounds];

        // Set origin point
        map[dungeon_bounds / 2, dungeon_bounds - 1] = OPEN; // The spawn room
        map[dungeon_bounds / 2, dungeon_bounds - 2] = UP; // The exit (only accessible from spawn room)

        int used_rooms = 0;
        // Repeat until enough rooms are placed
        while (used_rooms < max_rooms)
        {
            // Queueing phase
            for (int x = 0; x < dungeon_bounds; x++)
                for (int y = 0; y < dungeon_bounds; y++)
                    if (map[x, y] == OPEN)
                    {
                        map[x, y] = QUEUED;
                        used_rooms += 1;
                    }

            // Placing phase
            for (int x = 0; x < dungeon_bounds; x++)
                for (int y = 0; y < dungeon_bounds; y++)
                    if (map[x, y] == QUEUED)
                    {
                        int door_value = 0;

                        if (x != 0)
                        {
                            // This assumes all surrounding tiles are filled
                            // First check if tiles are filled, then do something
                            // else if they are not.
                            door_value += LEFT * (map[x - 1, y] / 1 % 2);
                        }
                        if (x != dungeon_bounds - 1)
                            door_value += RIGHT * (map[x + 1, y] / 2 % 2);
                        if (y != 0)
                            door_value += UP * (map[x, y - 1] / 4 % 2);
                        if (y != dungeon_bounds - 1)
                            door_value += DOWN * (map[x, y + 1] / 8 % 2);

                        map[x, y] = door_value;
                    }

            // 
        }
    }
}
