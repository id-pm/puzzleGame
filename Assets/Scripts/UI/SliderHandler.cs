using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderHandler : MonoBehaviour, IEndDragHandler, IPointerClickHandler
{
    public UnityEvent OnDragEnd;
    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();
    }
}
