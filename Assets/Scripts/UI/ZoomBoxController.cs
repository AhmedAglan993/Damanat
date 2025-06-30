using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Ricimi;

public class ZoomBoxController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform boxRect;
    public RectTransform leftHandle;
    public RectTransform rightHandle;
    public RectTransform contentArea;
    public TimelineUIController timelineController;

    private enum DragMode { None, Move, ResizeLeft, ResizeRight }
    private DragMode dragMode;
    private Vector2 dragStartMouse;
    private Vector2 dragStartBoxPos;
    private float dragStartWidth;

    void Start()
    {
        if (!timelineController)
            timelineController = FindObjectOfType<TimelineUIController>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(contentArea, eventData.position, eventData.pressEventCamera, out dragStartMouse);
        RectTransform rt = eventData.pointerPressRaycast.gameObject.GetComponent<RectTransform>();

        if (rt == leftHandle) dragMode = DragMode.ResizeLeft;
        else if (rt == rightHandle) dragMode = DragMode.ResizeRight;
        else dragMode = DragMode.Move;

        dragStartBoxPos = boxRect.anchoredPosition;
        dragStartWidth = boxRect.sizeDelta.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(contentArea, eventData.position, eventData.pressEventCamera, out Vector2 currentMouse);
        Vector2 delta = currentMouse - dragStartMouse;

        switch (dragMode)
        {
            case DragMode.Move:
                float newX = Mathf.Clamp(dragStartBoxPos.x + delta.x, 0, contentArea.rect.width - boxRect.sizeDelta.x);
                boxRect.anchoredPosition = new Vector2(newX, boxRect.anchoredPosition.y);
                break;

            case DragMode.ResizeLeft:
                float left = dragStartBoxPos.x + delta.x;
                float widthLeft = dragStartWidth - delta.x;
                if (widthLeft >= 60 && left >= 0)
                {
                    boxRect.anchoredPosition = new Vector2(left, boxRect.anchoredPosition.y);
                    boxRect.sizeDelta = new Vector2(widthLeft, boxRect.sizeDelta.y);
                }
                break;

            case DragMode.ResizeRight:
                float widthRight = dragStartWidth + delta.x;
                if (widthRight >= 60 && boxRect.anchoredPosition.x + widthRight <= contentArea.rect.width)
                {
                    boxRect.sizeDelta = new Vector2(widthRight, boxRect.sizeDelta.y);
                }
                break;
        }

        HighlightFramesUnderBox();
    }

    void HighlightFramesUnderBox()
    {
        float boxStart = boxRect.anchoredPosition.x;
        float boxEnd = boxStart + boxRect.sizeDelta.x;

        foreach (var frame in timelineController.GetFrames())
        {
            string label = frame.time.ToString(); // e.g., "10:00"
            if (timelineController.alertIcons.TryGetValue(label, out GameObject icon))
            {
                RectTransform iconRT = icon.GetComponent<RectTransform>();
                float x = iconRT.anchoredPosition.x;

                bool inRange = x >= boxStart && x <= boxEnd;
                icon.SetActive(inRange);
            }
        }
    }

}
