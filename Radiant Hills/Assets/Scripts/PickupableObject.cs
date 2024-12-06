using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    // Indicates whether the player is within range to pick up the object
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player (you can customize this tag)
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset the flag when the player exits the trigger
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    private void Update()
    {
        // Check for the "E" key press when in range
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        // Perform any additional logic for picking up the object here
        Debug.Log($"{gameObject.name} picked up!");

        // Disable the parent object from the scene (or destroy it if needed)
        // You can choose whether to disable or destroy the parent object based on your requirements
        if (transform.parent != null)
        {
            // To disable the parent object
            transform.parent.gameObject.SetActive(false);

            // Or if you want to destroy the parent object:
            // Destroy(transform.parent.gameObject);
        }
        else
        {
            // If the object has no parent, just disable this object
            gameObject.SetActive(false);
        }
    }
}