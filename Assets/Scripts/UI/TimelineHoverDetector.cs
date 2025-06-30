using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
