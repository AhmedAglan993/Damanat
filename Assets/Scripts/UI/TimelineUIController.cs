using Ricimi;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimelineUIController : MonoBehaviour
{
    [Header("References")]
    public RectTransform contentPanel;
    public GameObject timeFramePrefab;
    public AlertPopupManager alertPopupManager;
    private readonly Dictionary<string, Color> severityColors = new()
{
    { "Info", new Color(0.3f, 0.7f, 1f) },     // Light Blue
    { "Warning", new Color(1f, 0.65f, 0f) },   // Orange
    { "Critical", new Color(1f, 0.2f, 0.2f) }, // Red
};

    [Header("Zoom Settings")]
    public float minFrameWidth = 60f;
    public float zoomDuration = 0.35f;
    public float zoomCooldown = 0.2f;
    public float scrollThreshold = 0.4f;
    public RectTransform alertOverlayPanel; // Drag your new AlertOverlay here
    public GameObject alertIconPrefab;      // A small button/icon prefab
    private List<GameObject> activeAlertIcons = new(); // To track for cleanup
    private Dictionary<string, GameObject> iconDict = new();

    public TimeSpan startTime = new(10, 0, 0);
    public TimeSpan currentEndTime;
    public TimeSpan currentStep = TimeSpan.FromMinutes(1);

    public List<AlertEntry> alertEntries = new();

    private int visibleFrames = 60;
    private float lastZoomTime;

    private void Start()
    {
        currentEndTime = startTime + TimeSpan.FromMinutes(60);
        GenerateRandomAlerts(50);
        ZoomToTimeRange(startTime, currentEndTime);
    }
    public void GenerateRandomAlerts(int count)
    {
        string[] titles = {
        "Unauthorized Access", "Power Spike", "Sensor Offline", "Temperature Spike",
        "Door Left Open", "CO₂ Threshold Exceeded", "Fire Alarm Triggered",
        "Humidity Anomaly", "Elevator Error", "Vibration Detected"
    };

        string[] locations = {
        "Main Entrance - Zone A", "Server Room 2", "Cold Storage Zone 1", "Exit Corridor - East Wing",
        "Basement Level B3 - Pipe Room", "Roof Panel 3", "Elevator Shaft", "Lab 4 - South Wing"
    };

        string[] descriptions = {
        "Detected anomaly in system logs.",
        "Sensor lost connection for over 3 minutes.",
        "Temperature spike registered above 30°C.",
        "Access control flagged suspicious activity.",
        "Power voltage fluctuated beyond safe limit.",
        "Security system was manually overridden.",
        "Emergency door left open unexpectedly.",
        "Unscheduled equipment shutdown occurred."
    };

        string[] actions = {
        "Check device status manually.",
        "Dispatch technician to the location.",
        "Notify supervisor and document the event.",
        "Review surveillance for possible cause.",
        "Restart hardware or check cabling."
    };

        string[] types = { "Security", "Electrical", "Environment", "Access Control", "System" };
        string[] severities = { "Info", "Warning", "Critical" };

        System.Random rand = new();

        for (int i = 0; i < count; i++)
        {
            int hour = rand.Next(7, 18); // between 7 AM and 5 PM
            int minute = rand.Next(0, 60);
            string alertTime = new TimeSpan(hour, minute, 0).ToString(@"hh\:mm");

            alertEntries.Add(new AlertEntry
            {
                alertTitle = titles[rand.Next(titles.Length)],
                alertTime = alertTime,
                alertLocation = locations[rand.Next(locations.Length)],
                alertDescription = descriptions[rand.Next(descriptions.Length)],
                alertAction = actions[rand.Next(actions.Length)],
                alertType = types[rand.Next(types.Length)],
                alertSeverity = severities[rand.Next(severities.Length)]
            });
        }
    }
    public bool HasAlertFor(string timeLabel)
    {
        return alertEntries.Exists(a => a.alertTime == timeLabel);
    }


    private void Update()
    {
        if (!TimelineHoverDetector.isHovering) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < scrollThreshold) return;
        if (Time.time - lastZoomTime < zoomCooldown) return;

        lastZoomTime = Time.time;
        if (scroll > 0) ZoomIn();
        else ZoomOut();
    }

    void ZoomIn() => AdjustZoom(0.8f);
    void ZoomOut() => AdjustZoom(1.25f);

    void AdjustZoom(float factor)
    {
        TimeSpan currentRange = currentEndTime - startTime;
        double newRangeSeconds = Math.Max(10, currentRange.TotalSeconds * factor);

        TimeSpan newRange = TimeSpan.FromSeconds(newRangeSeconds);
        TimeSpan center = startTime + currentRange / 2;

        TimeSpan newStart = center - newRange / 2;
        TimeSpan newEnd = center + newRange / 2;

        ZoomToTimeRange(newStart, newEnd);
    }

    public void ZoomToTimeRange(TimeSpan start, TimeSpan end)
    {
        startTime = start;
        currentEndTime = end;

        double totalSeconds = (end - start).TotalSeconds;
        if (totalSeconds < 10) totalSeconds = 10;

        float contentWidth = contentPanel.rect.width;
        int maxFrames = Mathf.Max(5, Mathf.FloorToInt(contentWidth / minFrameWidth));
        visibleFrames = Mathf.Clamp(maxFrames, 5, 200);

        double stepSeconds = totalSeconds / visibleFrames;
        stepSeconds = Math.Max(1, stepSeconds); // Prevent zero
        currentStep = TimeSpan.FromSeconds(stepSeconds);

        GenerateTimeline();
    }
    public void UpdateAlertVisibility(TimeSpan visibleStart, TimeSpan visibleEnd)
    {
        foreach (var kvp in iconDict)
        {
            string label = kvp.Key;
            GameObject icon = kvp.Value;

            if (TimeSpan.TryParse(label, out var time))
            {
                bool shouldShow = time >= visibleStart && time <= visibleEnd;
                icon.SetActive(shouldShow);
            }
        }
    }

    public void GenerateTimeline()
    {
        ClearTimeline();

        float frameWidth = contentPanel.rect.width / visibleFrames;

        for (int i = 0; i < visibleFrames; i++)
        {
            TimeSpan time = startTime + TimeSpan.FromSeconds(i * currentStep.TotalSeconds);
            DateTime wallClock = DateTime.Today.Add(time);
            string timeLabel = currentStep.TotalSeconds >= 60 ? wallClock.ToString("HH:mm") : wallClock.ToString("mm:ss");

            GameObject tf = Instantiate(timeFramePrefab, contentPanel);
            tf.name = $"Frame_{timeLabel}";
            tf.GetComponent<RectTransform>().sizeDelta = new Vector2(frameWidth, 60f);
            tf.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = timeLabel;

            AlertEntry entry = alertEntries.Find(a => a.alertTime == timeLabel);
            if (entry != null)
            {
                GameObject icon = Instantiate(alertIconPrefab, alertOverlayPanel);
                icon.name = $"Alert_{timeLabel}";

                // Set color based on severity
                if (icon.TryGetComponent(out Image iconImage) && severityColors.TryGetValue(entry.alertSeverity, out Color color))
                {
                    iconImage.color = color;
                }

                icon.GetComponent<Button>().onClick.AddListener(() => alertPopupManager.ShowAlert(entry));

                RectTransform iconRect = icon.GetComponent<RectTransform>();
                iconRect.anchoredPosition = new Vector2(i * frameWidth, 0);
                icon.SetActive(false); // Initially hidden, shown by ZoomBox

                iconDict[timeLabel] = icon;
                activeAlertIcons.Add(icon);
            }

        }
    }
    void LateUpdate()
    {
        alertOverlayPanel.anchoredPosition = contentPanel.anchoredPosition;
    }

    public List<AlertEntry> GetAlertsInRange(TimeSpan start, TimeSpan end)
    {
        return alertEntries.FindAll(a =>
        {
            if (TimeSpan.TryParse(a.alertTime, out TimeSpan alertTS))
            {
                return alertTS >= start && alertTS <= end;
            }
            return false;
        });
    }

    void ClearTimeline()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        foreach (GameObject icon in activeAlertIcons)
            Destroy(icon);
        activeAlertIcons.Clear();
        iconDict.Clear();
    }

}

[System.Serializable]
public class AlertEntry
{
    public string alertTitle;
    public string alertTime;
    public string alertLocation;
    public string alertDescription;
    public string alertAction;
    public string alertType;
    public string alertSeverity;
}
