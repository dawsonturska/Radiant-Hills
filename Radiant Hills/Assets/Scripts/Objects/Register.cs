using UnityEngine;

public class RegisterTrigger : MonoBehaviour
{
    public Counter counter; // Reference to the Counter script

    private void Start()
    {
        if (counter == null)
        {
            counter = GetComponentInParent<Counter>();
            if (counter == null)
            {
                Debug.LogError("Counter reference not set on RegisterTrigger!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Register trigger.");
            counter.SetPlayerInTrigger(true); // FIXED
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited Register trigger.");
            counter.SetPlayerInTrigger(false); // FIXED
        }
    }
}
