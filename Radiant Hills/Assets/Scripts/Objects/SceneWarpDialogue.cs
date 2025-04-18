using UnityEngine;
using UnityEngine.Events;

public class SceneWarpDialogue : MonoBehaviour
{
    [Tooltip("Event that will be called when the E key is pressed while the player is in range.")]
    public UnityEvent onInteract;

    public GameObject indicatorPrefab;

    private GameObject activeIndicator;
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            ShowIndicator();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            HideIndicator();
        }
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            onInteract?.Invoke();
        }
    }

    private void ShowIndicator()
    {
        if (indicatorPrefab != null && activeIndicator == null)
        {
            activeIndicator = Instantiate(indicatorPrefab, transform.position + Vector3.up, Quaternion.identity);
            activeIndicator.transform.SetParent(transform);
        }
    }

    private void HideIndicator()
    {
        if (activeIndicator != null)
        {
            Destroy(activeIndicator);
        }
    }
}
