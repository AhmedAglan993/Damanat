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
    private List<LineRenderer> animatedLines = new();
    private HashSet<string> drawnSegments = new(); // e.g., "A-B" or "B-A"

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
        foreach (var line in animatedLines)
        {
            if (line == null) continue;
            float offset = Time.time * -arrowAnimSpeed;
            line.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }

    public void EvacuateAll()
    {
        ClearPath();

        EmergencyRoomTrigger[] allRooms = FindObjectsOfType<EmergencyRoomTrigger>();
        foreach (var room in allRooms)
        {
            ShowEvacuationPathFrom(room.transform);
        }
    }
    private void ShowEvacuationPathFrom(Transform startTransform)
    {
        var path = EmergencyPathfinder.Instance.GetPath(startTransform);
        if (path == null || path.Count == 0) return;

        // Spawn markers
        foreach (var node in path)
        {
            GameObject marker = Instantiate(pathMarkerPrefab, node.transform.position + Vector3.up * 0.2f, Quaternion.identity);
            markers.Add(marker);
        }

        // Draw interpolated line

        for (int i = 0; i < path.Count - 1; i++)
        {
            EmergencyNode nodeA = path[i];
            EmergencyNode nodeB = path[i + 1];

            string key1 = $"{nodeA.GetInstanceID()}-{nodeB.GetInstanceID()}";
            string key2 = $"{nodeB.GetInstanceID()}-{nodeA.GetInstanceID()}";

            if (drawnSegments.Contains(key1) || drawnSegments.Contains(key2))
                continue;

            drawnSegments.Add(key1);

            Vector3 start = nodeA.transform.position + Vector3.up * 0.2f;
            Vector3 end = nodeB.transform.position + Vector3.up * 0.2f;

            List<Vector3> interpolatedPoints = new();
            interpolatedPoints.Add(start);
            int stepCount = 5;
            for (int j = 1; j < stepCount; j++)
            {
                float t = j / (float)stepCount;
                interpolatedPoints.Add(Vector3.Lerp(start, end, t));
            }
            interpolatedPoints.Add(end);

            GameObject lineObj = new GameObject($"PathLine_{nodeA.name}_to_{nodeB.name}");
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.material = new Material(pathLineMaterial); // separate instance
            line.widthMultiplier = lineWidth;
            line.positionCount = interpolatedPoints.Count;
            line.SetPositions(interpolatedPoints.ToArray());
            line.material.mainTextureScale = new Vector2(4f, 1f); // adjust if needed
            animatedLines.Add(line);
            markers.Add(lineObj);
        }
    }

    public void ShowExitPathFrom(Transform user)
    {
        ClearPath();
        ShowEvacuationPathFrom(user);
    }


    public void ClearPath()
    {
        foreach (var obj in markers)
            if (obj != null) Destroy(obj);

        markers.Clear();
        animatedLines.Clear();
        drawnSegments.Clear();
    }

}
