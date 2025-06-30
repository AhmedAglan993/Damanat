using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Ricimi;

public class TimelineUIController : MonoBehaviour
{
    [Header("References")]
    public RectTransform contentPanel;
    public GameObject timeFramePrefab;
    public AlertPopupManager alertPopupManager;

    [Header("Zoom Settings")]
    public float frameWidth = 60f;
    public int startHour = 0;
    public int endHour = 24;
    [Header("Alert Overlay")]
    public RectTransform alertOverlayPanel;       // Assign this in the inspector
    public GameObject alertIconPrefab;            // The alert button prefab for overlay layer

    public Dictionary<string, GameObject> alertIcons = new(); // key: "HH:mm"
    private readonly Dictionary<string, Color> severityColors = new()
    {
        { "Info", new Color(0.3f, 0.7f, 1f) },     // Light Blue
        { "Warning", new Color(1f, 0.65f, 0f) },   // Orange
        { "Critical", new Color(1f, 0.2f, 0.2f) }, // Red
    };
    public List<TimelineFrame> frames = new();
    private ZoomLevel currentZoom = ZoomLevel.Hour;

    private readonly Dictionary<ZoomLevel, int> zoomSteps = new()
    {
        { ZoomLevel.Hour, 60 },
        { ZoomLevel.HalfHour, 30 },
        { ZoomLevel.QuarterHour, 15 },
        { ZoomLevel.FiveMinutes, 5 },
        { ZoomLevel.OneMinute, 1 },
    };

    public List<AlertEntry> alertEntries = new();

    void Start()
    {
        GenerateRandomAlerts(80);
        GenerateBaseTimeline();
    }
    void GenerateRandomAlerts(int count)
    {
        string[] titles = { "Unauthorized Access", "Power Spike", "Sensor Offline", "Door Open" };
        string[] locations = { "Main Entrance", "Server Room", "Cold Storage", "Basement" };
        string[] severities = { "Info", "Warning", "Critical" };

        System.Random rand = new();

        for (int i = 0; i < count; i++)
        {
            int randomMinute = rand.Next(startHour * 60, endHour * 60);
            ClockTime time = ClockTime.FromTotalMinutes(randomMinute);

            alertEntries.Add(new AlertEntry
            {
                alertTitle = titles[rand.Next(titles.Length)],
                alertTime = time.ToString(), // "HH:mm"
                alertLocation = locations[rand.Next(locations.Length)],
                alertDescription = "Random alert generated for testing.",
                alertAction = "Check system logs.",
                alertType = "System",
                alertSeverity = severities[rand.Next(severities.Length)]
            });
        }
    }

    void Update()
    {
        if (!TimelineHoverDetector.isHovering) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.1f)
        {
            if (scroll > 0 && currentZoom < ZoomLevel.OneMinute)
            {
                currentZoom++;
                InsertZoomFrames();
            }
            else if (scroll < 0 && currentZoom > ZoomLevel.Hour)
            {
                currentZoom--;
                if (currentZoom == ZoomLevel.Hour)
                    RebuildBaseTimeline();
            }
        }
    }

    void GenerateBaseTimeline()
    {
        frames.Clear();
        for (int h = startHour; h < endHour; h++)
        {
            ClockTime time = new ClockTime(h, 0);
            frames.Add(CreateFrame(time));
            AlertEntry match = alertEntries.Find(a => a.alertTime == time.ToString());
            if (match != null)
                CreateAlertIcon(time, match);
        }
        RepositionFrames();
        RepositionAlertIcons();
    }

    void RebuildBaseTimeline()
    {
        foreach (var f in frames)
            Destroy(f.instance);
        GenerateBaseTimeline();
    }

    void InsertZoomFrames()
    {
        int step = zoomSteps[currentZoom];
        List<TimelineFrame> newFrames = new();

        for (int i = 0; i < frames.Count - 1; i++)
        {
            ClockTime t1 = frames[i].time;
            ClockTime t2 = frames[i + 1].time;

            int gap = t2.TotalMinutes - t1.TotalMinutes;
            if (gap > step)
            {
                for (int m = step; m < gap; m += step)
                {
                    ClockTime tInsert = t1 + m;

                    // Avoid duplicates
                    if (!frames.Exists(f => f.time.TotalMinutes == tInsert.TotalMinutes))
                    {
                        TimelineFrame inserted = CreateFrame(tInsert);
                        newFrames.Add(inserted);
                    }
                    if (alertEntries.Exists(a => a.alertTime == tInsert.ToString()))
                    {
                        AlertEntry match = alertEntries.Find(a => a.alertTime == tInsert.ToString());
                        CreateAlertIcon(tInsert, match);
                    }

                }
            }
        }

        // Add and sort by time
        frames.AddRange(newFrames);
        frames.Sort((a, b) => a.time.TotalMinutes.CompareTo(b.time.TotalMinutes));

        RepositionFrames();
        RepositionAlertIcons();
    }


    TimelineFrame CreateFrame(ClockTime time)
    {
        GameObject tf = Instantiate(timeFramePrefab, contentPanel);
        tf.name = $"Frame_{time}";
        tf.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = time.ToString();
        tf.GetComponent<RectTransform>().sizeDelta = new Vector2(frameWidth, 60f);
        RepositionAlertIcons();
        return new TimelineFrame { time = time, instance = tf };

    }
    void CreateAlertIcon(ClockTime time, AlertEntry entry)
    {
        GameObject icon = Instantiate(alertIconPrefab, alertOverlayPanel);
        icon.name = $"Alert_{time}";
        if (icon.TryGetComponent(out Image iconImage) && severityColors.TryGetValue(entry.alertSeverity, out Color color))
        {
            iconImage.color = color;
            icon.GetComponent<Button>().onClick.AddListener(() => alertPopupManager.ShowAlert(entry, color));
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10); // match size
        }


        alertIcons[time.ToString()] = icon;
    }
    void LateUpdate()
    {
        alertOverlayPanel.anchoredPosition = contentPanel.anchoredPosition;
    }
    void RepositionAlertIcons()
    {
        foreach (var frame in frames)
        {
            string label = frame.time.ToString(); // "HH:mm"

            if (alertIcons.TryGetValue(label, out GameObject icon))
            {
                RectTransform frameRect = frame.instance.GetComponent<RectTransform>();
                RectTransform iconRect = icon.GetComponent<RectTransform>();

                // Use same X as frame, keep alert Y fixed (e.g. center or top)
                iconRect.anchoredPosition = new Vector2(frameRect.anchoredPosition.x + 30, -10);
            }
        }
    }


    void RepositionFrames()
    {
        for (int i = 0; i < frames.Count; i++)
        {
            TimelineFrame frame = frames[i];
            RectTransform rt = frame.instance.GetComponent<RectTransform>();
            float xPos = i * frameWidth;

            // Set anchored position based on index
            rt.anchoredPosition = new Vector2(i * frameWidth, 0);

            // Ensure visual order in the hierarchy matches list order
            frame.instance.transform.SetSiblingIndex(i);
            if (alertIcons.TryGetValue(frame.time.ToString(), out GameObject alertGO))
            {
                alertGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0);
            }
        }
        RepositionAlertIcons();


        // Resize the scroll area
        contentPanel.sizeDelta = new Vector2(frames.Count * frameWidth, contentPanel.sizeDelta.y);
        alertOverlayPanel.sizeDelta = contentPanel.sizeDelta;

    }




    public List<TimelineFrame> GetFrames() => frames;
}

public enum ZoomLevel
{
    Hour = 0,
    HalfHour = 1,
    QuarterHour = 2,
    FiveMinutes = 3,
    OneMinute = 4
}

[System.Serializable]
public class AlertEntry
{
    public string alertTitle;
    public string alertTime; // "HH:mm"
    public string alertLocation;
    public string alertDescription;
    public string alertAction;
    public string alertType;
    public string alertSeverity;
}
[System.Serializable]
public class TimelineFrame
{
    public ClockTime time;
    public GameObject instance;
}
