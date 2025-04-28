using UnityEngine;
using UnityEngine.Events;

public class RandomEventTrigger : MonoBehaviour
{
    [Header("List of Events to Choose From")]
    public UnityEvent[] randomEvents;

    // Call this method to trigger a random event
    public void TriggerRandomEvent()
    {
        if (randomEvents == null || randomEvents.Length == 0)
        {
            Debug.LogWarning("No events assigned to RandomEventTrigger on " + gameObject.name);
            return;
        }

        int randomIndex = Random.Range(0, randomEvents.Length);
        randomEvents[randomIndex]?.Invoke();
    }
}
