using Ricimi;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomBoxController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform boxRect;
    public RectTransform leftHandle;
    public RectTransform rightHandle;
    public RectTransform contentArea;
    public TimelineUIController timelineController;

    public float minWidth = 60f;

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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            contentArea, eventData.position, eventData.pressEventCamera, out dragStartMouse
        );

        var rt = eventData.pointerPressRaycast.gameObject.GetComponent<RectTransform>();
        dragMode = rt == leftHandle ? DragMode.ResizeLeft :
                   rt == rightHandle ? DragMode.ResizeRight :
                   DragMode.Move;

        dragStartBoxPos = boxRect.anchoredPosition;
        dragStartWidth = boxRect.sizeDelta.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            contentArea, eventData.position, eventData.pressEventCamera, out Vector2 currentMouse
        );

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
                if (widthLeft >= minWidth && left >= 0)
                {
                    boxRect.anchoredPosition = new Vector2(left, boxRect.anchoredPosition.y);
                    boxRect.sizeDelta = new Vector2(widthLeft, boxRect.sizeDelta.y);
                }
                break;

            case DragMode.ResizeRight:
                float widthRight = dragStartWidth + delta.x;
                if (widthRight >= minWidth && boxRect.anchoredPosition.x + widthRight <= contentArea.rect.width)
                {
                    boxRect.sizeDelta = new Vector2(widthRight, boxRect.sizeDelta.y);
                }
                break;
        }

       // ShowAlertsUnderBox();
    }
    void Update()
    {
        if (dragMode != DragMode.None)
        {
            var (start, end) = GetSelectedTimeRange();
            timelineController.UpdateAlertVisibility(start, end);
        }
    }

    void ShowAlertsUnderBox()
    {
        var (start, end) = GetSelectedTimeRange();

        for (int i = 0; i < timelineController.contentPanel.childCount; i++)
        {
            Transform frame = timelineController.contentPanel.GetChild(i);
            string label = frame.Find("Label").GetComponent<TextMeshProUGUI>().text;

            if (!TimeSpan.TryParse(label, out var labelTime))
                continue;

            CleanButton alertBtn = frame.Find("AlertIcon").GetComponent<CleanButton>();
            bool isInRange = labelTime >= start && labelTime <= end;

            alertBtn.gameObject.SetActive(isInRange && timelineController.HasAlertFor(label));
        }
    }

    public (TimeSpan start, TimeSpan end) GetSelectedTimeRange()
    {
        float timelineSeconds = (float)(timelineController.currentEndTime - timelineController.startTime).TotalSeconds;
        float contentWidth = contentArea.rect.width;

        float pixelsPerSecond = timelineSeconds > 0 ? contentWidth / timelineSeconds : 1f;
        float startSec = boxRect.anchoredPosition.x / pixelsPerSecond;
        float durationSec = boxRect.sizeDelta.x / pixelsPerSecond;

        TimeSpan start = timelineController.startTime + TimeSpan.FromSeconds(Mathf.Max(0, startSec));
        TimeSpan end = start + TimeSpan.FromSeconds(Mathf.Max(1, durationSec));
        return (start, end);
    }
   

}
