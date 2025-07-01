using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherGraphRenderer : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject pointPrefab;
    public Color lineColor = Color.cyan;
    public float graphHeight = 100f;
    public float graphWidth = 300f;

    private List<GameObject> points = new();

    public void DrawGraph(List<float> dataPoints)
    {
        ClearGraph();

        if (dataPoints.Count == 0) return;

        float xStep = graphWidth / (dataPoints.Count - 1);
        float maxValue = Mathf.Max(dataPoints.ToArray());

        for (int i = 0; i < dataPoints.Count; i++)
        {
            float normalizedY = dataPoints[i] / maxValue;
            float xPos = i * xStep;
            float yPos = normalizedY * graphHeight;

            GameObject point = Instantiate(pointPrefab, graphContainer);
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            points.Add(point);

            // Optional: draw lines
            if (i > 0)
            {
                DrawLine(points[i - 1].GetComponent<RectTransform>().anchoredPosition, new Vector2(xPos, yPos));
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(graphContainer, false);
        lineObj.GetComponent<Image>().color = lineColor;
        RectTransform rt = lineObj.GetComponent<RectTransform>();
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);
        rt.sizeDelta = new Vector2(distance, 3f);
        rt.anchoredPosition = start + direction * distance * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        points.Add(lineObj);
    }

    void ClearGraph()
    {
        foreach (var obj in points)
            Destroy(obj);
        points.Clear();
    }
}
