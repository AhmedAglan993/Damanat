using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeatherGraphRenderer : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    public TMP_Text[] xAxisLabels; // Labels for each day under the graph
    public TMP_Text yAxisMinLabel, yAxisMaxLabel;

    public Color tempColor = Color.red;
    public Color rainColor = Color.cyan;
    public Color windColor = Color.green;

    private WeatherGraphMode currentMode = WeatherGraphMode.Temperature;
    private List<GameObject> pointObjects = new();
    private List<GameObject> lineObjects = new();

    private List<WeatherData> forecast = new();
    private void Awake()
    {
        SwitchMode("Wind");
    }
    public void SetForecast(List<WeatherData> data)
    {
        forecast = data;
        UpdateGraph();
    }

    public void SwitchMode(string mode)
    {
        if (Enum.TryParse(mode, out WeatherGraphMode parsed))
        {
            currentMode = parsed;
            UpdateGraph();
        }
    }

    private void UpdateGraph()
    {
        ClearGraph();

        if (forecast == null || forecast.Count == 0)
            return;

        float graphHeight = graphContainer.rect.height;
        float graphWidth = graphContainer.rect.width;
        float xSpacing = graphWidth / (forecast.Count - 1);

        // Get Y range
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        List<float> values = new();

        foreach (var day in forecast)
        {
            float v = GetValue(day);
            values.Add(v);
            minY = Mathf.Min(minY, v);
            maxY = Mathf.Max(maxY, v);
        }

        minY = Mathf.Floor(minY);
        maxY = Mathf.Ceil(maxY);
        float yRange = Mathf.Max(maxY - minY, 1f); // prevent div 0

        yAxisMinLabel.text = minY.ToString();
        yAxisMaxLabel.text = maxY.ToString();

        Vector2? prevPos = null;

        for (int i = 0; i < values.Count; i++)
        {
            float x = xSpacing * i;
            float y = ((values[i] - minY) / yRange) * graphHeight;

            GameObject point = Instantiate(pointPrefab, graphContainer);
            point.GetComponent<Image>().color = GetModeColor();
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            pointObjects.Add(point);
            if (i < xAxisLabels.Length)
            {
                xAxisLabels[i].text = DateTime.Now.AddDays(i).ToString("ddd");
            }

            if (prevPos != null)
            {
                GameObject line = Instantiate(linePrefab, graphContainer);
                DrawLine(line.GetComponent<RectTransform>(), prevPos.Value, new Vector2(x, y));
                line.GetComponent<Image>().color = GetModeColor();
                lineObjects.Add(line);
            }

            prevPos = new Vector2(x, y);
        }
    }

    private void DrawLine(RectTransform rt, Vector2 start, Vector2 end)
    {
        Vector2 dir = (end - start).normalized;
        float dist = Vector2.Distance(start, end);
        rt.sizeDelta = new Vector2(dist, 5f);
        rt.anchoredPosition = start + dir * dist / 2;
        rt.rotation = Quaternion.FromToRotation(Vector3.right, end - start);
    }

    private float GetValue(WeatherData data)
    {
        return currentMode switch
        {
            WeatherGraphMode.Temperature => data.temperature,
            WeatherGraphMode.Precipitation => data.precipitation,
            WeatherGraphMode.Wind => data.windSpeed,
            _ => 0f
        };
    }

    private Color GetModeColor()
    {
        return currentMode switch
        {
            WeatherGraphMode.Temperature => tempColor,
            WeatherGraphMode.Precipitation => rainColor,
            WeatherGraphMode.Wind => windColor,
            _ => Color.white
        };
    }

    private void ClearGraph()
    {
        foreach (var go in pointObjects) Destroy(go);
        foreach (var go in lineObjects) Destroy(go);
        pointObjects.Clear();
        lineObjects.Clear();
    }
}

public enum WeatherGraphMode
{
    Temperature,
    Precipitation,
    Wind
}

[System.Serializable]
public class DailyWeatherData
{
    public float temperature;
    public float precipitation;
    public float windSpeed;
}
