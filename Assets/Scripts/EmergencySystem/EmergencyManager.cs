using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmergencyManager : MonoBehaviour
{
    public static EmergencyManager Instance { get; private set; }
    public GameObject pathMarkerPrefab;
    public Material pathLineMaterial;
    public float lineWidth = 0.4f;
    public Transform emergencyArea;
    private List<GameObject> markers = new();
    private LineRenderer pathLine;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
    }
    private float arrowAnimSpeed = 1.5f;
    private float arrowOffset = 0f;

    private void Update()
    {
        if (pathLine != null)
        {
            arrowOffset -= Time.deltaTime * arrowAnimSpeed;
            pathLine.material.SetTextureOffset("_MainTex", new Vector2(arrowOffset, 0));
        }
    }
    public void ShowExitPathFrom(Transform user)
    {
        ClearPath();

        var path = EmergencyPathfinder.Instance.GetPath(user);
        if (path == null || path.Count == 0) return;

        // Spawn markers
        foreach (var node in path)
        {
            GameObject marker = Instantiate(pathMarkerPrefab, node.transform.position + Vector3.up * 0.2f, Quaternion.identity);
            markers.Add(marker);
        }

        // Draw line between nodes
        GameObject lineObj = new GameObject("PathLine");
        pathLine = lineObj.AddComponent<LineRenderer>();
        pathLine.material = pathLineMaterial;
        pathLine.widthMultiplier = lineWidth;
        List<Vector3> interpolatedPoints = new();

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 start = path[i].transform.position + Vector3.up * 0.2f;
            Vector3 end = path[i + 1].transform.position + Vector3.up * 0.2f;

            interpolatedPoints.Add(start); // always include the start

            // Add points between nodes (tweak "stepCount" for smoother lines)
            int stepCount = 5;
            for (int j = 1; j < stepCount; j++)
            {
                float t = j / (float)stepCount;
                Vector3 point = Vector3.Lerp(start, end, t);
                interpolatedPoints.Add(point);
            }
        }

        // Add the final point
        interpolatedPoints.Add(path[^1].transform.position + Vector3.up * 0.2f);

        // Now apply to LineRenderer
        pathLine.positionCount = interpolatedPoints.Count;
        pathLine.material.mainTextureScale = new Vector2(path.Count * 2f, 1f);

        print(interpolatedPoints.Count);
        pathLine.SetPositions(interpolatedPoints.ToArray());


        markers.Add(lineObj); // Include line in cleanup list
    }

    public void ClearPath()
    {
        foreach (var obj in markers)
            if (obj != null) Destroy(obj);

        markers.Clear();
    }
}
