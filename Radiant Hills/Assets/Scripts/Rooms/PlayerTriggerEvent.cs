using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerEvent : MonoBehaviour
{
    [Tooltip("The event to run when the player overlaps with this trigger")]
    public UnityEvent onPlayerTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerTrigger.Invoke();
        }
    }
}
