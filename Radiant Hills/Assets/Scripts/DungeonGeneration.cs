using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    // Dungeon layout
    public int[,] map;

    // Values used to fill tiles of "map"
    public int
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
        LEFT  = 1,    LEFT_BIT  = 0,
        RIGHT = 2,    RIGHT_BIT = 1,
        UP    = 4,    UP_BIT    = 2,
        DOWN  = 8,    DOWN_BIT  = 3,
        
        // SPECIAL ROOMS (No door bits)
        BOSS_ROOM = 16;

    public int dungeon_bounds = 10;

    // Start is called before the first frame update
    public void Start()
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


    // Use these getters and setters 
    // to access the door bits of a tile.

    // Ex. GetBit(map[2,4], UP_BIT);
    //      - Returns whether the tile
    //        (2, 4) has a door leading
    //        upward.
    public bool GetBit(int number, int bit)
    {
        return number / (int)Math.Pow(2, bit) % 2 == 1;
    }

    // Ex. map[2,4] = SetBit(map[2,4], UP_BIT) 
    //      - Declares that the tile
    //        (2, 4) has a door leading
    //        upward.
    public int SetBit(int number, int bit, bool value)
    {
        return (int)( (int) (number / Math.Pow(2, bit + 1)) * (int)Math.Pow(2, bit + 1) + Math.Pow(2, bit) * (value ? 1 : 0) + number % (int)Math.Pow(2, bit) );
    }

    public void Generate(int max_rooms = 10, int seed = 0)
    {
        Random random = new Random();
		bool is_valid = false;
        
        while (!is_valid)
        {
            // Initialize dungeon grid
            map = new int[dungeon_bounds, dungeon_bounds];

            // Set origin point
            map[dungeon_bounds / 2, dungeon_bounds - 2] = OPEN; // The spawn room
            map[dungeon_bounds / 2, dungeon_bounds - 1] = UP; // The exit (only accessible from spawn room)

            int used_rooms = 0;
            // Repeat until enough rooms are placed
            while (used_rooms < max_rooms)
            {
            	int rooms_added = 0;
                // Queueing phase
                // Open tiles are changed to queued tiles
                for (int x = 0; x < dungeon_bounds; x++)
                    for (int y = 0; y < dungeon_bounds; y++)
                        if (map[x, y] == OPEN && used_rooms < max_rooms)
                        {
                            map[x, y] = QUEUED;
                            used_rooms += 1;
                            rooms_added += 1;
                            if (used_rooms == max_rooms)
                            	map[x, y] = BOSS_ROOM;
                        }
                if (rooms_added == 0)
                	break;

                // Placing phase
                // Queued tiles are changed to filled tiles
                for (int x = 0; x < dungeon_bounds; x++)
                    for (int y = 0; y < dungeon_bounds; y++)
                        if (map[x, y] == QUEUED)
                        {
                            int door_value = 0;

                            if (x != 0)
                            {
                                if (map[x - 1, y] > 0)
                                {
                                    if (GetBit(map[x - 1, y], RIGHT_BIT))
                                        door_value += LEFT;
                                }
                                else if (random.Next(3) != 0)
                                {
                                    door_value += LEFT;
                                    map[x - 1, y] = OPEN;
                                }

                            }
                            if (x != dungeon_bounds - 1)
                            {
                                if (map[x + 1, y] > 0)
                                {
                                    if (GetBit(map[x + 1, y], LEFT_BIT))
                                        door_value += RIGHT;
                                }
                                else if (random.Next(3) != 0)
                                {
                                    door_value += RIGHT;
                                    map[x + 1, y] = OPEN;
                                }
                            }
                            if (y != 0)
                            {
                                if (map[x, y - 1] > 0)
                                {
                                    if (GetBit(map[x, y - 1], DOWN_BIT))
                                        door_value += UP;
                                }
                                else if (random.Next(3) != 0)
                                {
                                    door_value += UP;
                                    map[x, y - 1] = OPEN;
                                }
                            }
                            if (y != dungeon_bounds - 1)
                            {
                                if (map[x, y + 1] > 0)
                                {
                                    if (GetBit(map[x, y + 1], UP_BIT))
                                        door_value += DOWN;
                                }
                                else if (random.Next(3) != 0)
                                {
                                    door_value += DOWN;
                                    map[x, y + 1] = OPEN;
                                }
                            }

                            map[x, y] = door_value;
                        }
            	
            }
            is_valid = used_rooms == max_rooms;
        }

        // Cleanup phase
        // Remove any doors on the outside of the level
        // that don't lead anywhere
        for (int x = 0; x < dungeon_bounds; x++)
            for (int y = 0; y < dungeon_bounds; y++)
                if (map[x, y] > 0 && map[x, y] < 16)
                {
                    if (x != 0)
                    {
                        map[x, y] = SetBit(map[x, y], LEFT_BIT, GetBit(map[x, y], LEFT_BIT) && map[x - 1, y] > 0);
                    }
                    if (x != dungeon_bounds - 1)
                    {
                        map[x, y] = SetBit(map[x, y], RIGHT_BIT, GetBit(map[x, y], RIGHT_BIT) && map[x + 1, y] > 0);
                    }
                    if (y != 0)
                    {
                        map[x, y] = SetBit(map[x, y], UP_BIT, GetBit(map[x, y], UP_BIT) && map[x, y - 1] > 0);
                    }
                    if (y != dungeon_bounds - 1)
                    {
                        map[x, y] = SetBit(map[x, y], DOWN_BIT, GetBit(map[x, y], DOWN_BIT) && map[x, y + 1] > 0);
                    }
                }
        
        PrintDungeon();
    }

    public void PrintDungeon()
    {
        Console.WriteLine("Dungeon Layout:");

        // Find the first row of the dungeon that is
        // filled so that we don't waste space printing the
        // empty rows above that.
        int top;
        bool found = false;
        for (top = 0; top < dungeon_bounds; top++)
        {
            for (int x = 0; x < dungeon_bounds; x++)
                if (map[x, top] > 0)
                {
                    found = true;
                    break;
                }
            if (found)
                break;
        }

        for (int y = top; y < dungeon_bounds; y++)
        {
            string line = "";
            for (int x = 0; x < dungeon_bounds; x++)
            {
                line += " ";
                line += (map[x, y] > 0 && GetBit(map[x, y], 2)) ? "|" : " ";
                line += " ";
            }
            Console.WriteLine(line);
            line = "";
            
            for (int x = 0; x < dungeon_bounds; x++)
            {
                line += (map[x, y] > 0 && GetBit(map[x, y], 0)) ? "-" : " ";
                if (x == dungeon_bounds / 2 && y == dungeon_bounds - 2)
                	line += "S";
                else if (x == dungeon_bounds / 2 && y == dungeon_bounds - 1)
                	line += " ";
                else if (map[x, y] == BOSS_ROOM)
                	line += "B";
                else
	                line += (map[x, y] > 0) ? "#" : " ";
                line += (map[x, y] > 0 && GetBit(map[x, y], 1)) ? "-" : " ";
            }
            Console.WriteLine(line);
            line = "";
               
            for (int x = 0; x < dungeon_bounds; x++)
            {
                line += " ";
                line += (map[x, y] > 0 && GetBit(map[x, y], 3)) ? "|" : " ";
                line += " ";
            }
            Console.WriteLine(line);
        }
    }
}
