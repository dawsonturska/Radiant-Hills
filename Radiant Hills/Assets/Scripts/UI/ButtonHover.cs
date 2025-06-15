using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private UnityEvent onHover;

    private bool isHover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHover)
        {
            onHover?.Invoke();
            isHover = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!isHover)
        {
            onHover?.Invoke();
            isHover = true;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isHover = false;
    }
}