using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEvent : MonoBehaviour, IPointerEnterHandler
{
    // You can assign a method in the inspector or use this as a placeholder
    [SerializeField] private UnityEngine.Events.UnityEvent onHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered button area.");
        onHover?.Invoke();
    }
}
