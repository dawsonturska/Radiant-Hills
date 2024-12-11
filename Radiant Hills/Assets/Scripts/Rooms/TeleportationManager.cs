using UnityEngine;

public class TeleportationManager : MonoBehaviour
{
    [Tooltip("Tag assigned to room GameObjects")]
    public string roomTag = "Room"; // The tag used for room GameObjects

    [Tooltip("Maximum distance to link doors")]
    public float maxLinkDistance = 10f; // Maximum distance to link doors

    private void Start()
    {
        AutoLinkDoors(); // Link doors when the game starts
    }

    // Link all doors across rooms
    private void AutoLinkDoors()
    {
        // Find all room GameObjects by their tag
        GameObject[] rooms = GameObject.FindGameObjectsWithTag(roomTag);

        // Iterate over each room to link doors
        foreach (GameObject room in rooms)
        {
            // Find all doors that are children of the current room
            Door[] doorsInRoom = room.GetComponentsInChildren<Door>();

            // Iterate over each door in the current room
            foreach (Door door in doorsInRoom)
            {
                // Find and link the closest aligned door in a different room
                Door alignedDoor = FindAlignedClosestDoor(door, rooms);
                if (alignedDoor != null)
                {
                    // Link the door to the aligned door
                    door.targetDoor = alignedDoor.transform;
                    door.teleportOffset = CalculateOffset(door.transform, alignedDoor.transform);
                }
            }
        }
    }

    // Find the closest aligned door to the current door
    private Door FindAlignedClosestDoor(Door currentDoor, GameObject[] rooms)
    {
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;

        // Loop through all rooms
        foreach (GameObject room in rooms)
        {
            // Find all doors in the current room
            Door[] doorsInRoom = room.GetComponentsInChildren<Door>();

            // Check each door in this room
            foreach (Door door in doorsInRoom)
            {
                // Skip the current door (we don't want to link a door to itself)
                if (door == currentDoor) continue;

                // Calculate the distance between the current door and this door
                float distance = Vector2.Distance(currentDoor.transform.position, door.transform.position);

                // Check if the door is within the max link distance and is aligned on the same axis
                bool isAligned = Mathf.Abs(currentDoor.transform.position.x - door.transform.position.x) < 0.1f ||
                                 Mathf.Abs(currentDoor.transform.position.y - door.transform.position.y) < 0.1f;

                // If the door is the closest and satisfies the conditions, set it as the closest
                if (distance < closestDistance && distance <= maxLinkDistance && isAligned)
                {
                    closestDistance = distance;
                    closestDoor = door;
                }
            }
        }

        return closestDoor;
    }

    // Calculate the teleportation offset between two doors
    private Vector2 CalculateOffset(Transform fromDoor, Transform toDoor)
    {
        return toDoor.position - fromDoor.position; // Calculate offset based on the positions of the two doors
    }
}
