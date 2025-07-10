using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class StreetPathfinding : MonoBehaviour
{
    public static StreetPathfinding Instance;

    public enum PathStyle { Line, Arrow, Glow }
    [Header("Path Rendering")]
    public PathStyle currentStyle = PathStyle.Line;
    public Material lineMaterial;
    public Material arrowMaterial;
    public Material glowMaterial;
    public float pathHeightOffset = 0.2f;
    public float lineWidth = 0.4f;

    private LineRenderer lineRenderer;
   [SerializeField] private List<StreetNode> streetNodes = new();

    private void Awake()
    {
        Instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
    }

    private void Start()
    {
        CollectStreetNodes();
    }
    private float arrowAnimSpeed = 1.5f;
    private float arrowOffset = 0f;

    private void Update()
    {
        if (currentStyle == PathStyle.Arrow && lineRenderer.material != null)
        {
            arrowOffset -= Time.deltaTime * arrowAnimSpeed;
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(arrowOffset, 0));
        }
    }

    /// <summary>
    /// Automatically finds all StreetNode components in the scene
    /// </summary>
    private void CollectStreetNodes()
    {
        streetNodes.Clear();
        streetNodes.AddRange(FindObjectsOfType<StreetNode>());
    }

    /// <summary>
    /// Call this to draw the path between any two Transforms.
    /// </summary>
    public void ShowPath(Transform from, Transform to)
    {
        List<Vector3> path = FindPath(from.position, to.position);

        if (path.Count == 0)
        {
            Debug.LogWarning("No path found.");
            return;
        }
        print(path.Count);
        lineRenderer.material = GetMaterialForStyle();
        lineRenderer.positionCount = path.Count;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.material.mainTextureScale = new Vector2(path.Count * 2f, 1f);

        for (int i = 0; i < path.Count; i++)
            lineRenderer.SetPosition(i, path[i] + Vector3.up * pathHeightOffset);
    }

    /// <summary>
    /// Finds the shortest path using basic Dijkstra’s algorithm.
    /// </summary>
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        StreetNode startNode = FindClosestNode(startPos);
        StreetNode endNode = FindClosestNode(endPos);

        var openSet = new List<StreetNode> { startNode };
        var cameFrom = new Dictionary<StreetNode, StreetNode>();
        var cost = new Dictionary<StreetNode, float> { [startNode] = 0 };

        while (openSet.Count > 0)
        {
            StreetNode current = openSet[0];
            foreach (var node in openSet)
                if (cost[node] < cost[current]) current = node;

            if (current == endNode)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                float newCost = cost[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (!cost.ContainsKey(neighbor) || newCost < cost[neighbor])
                {
                    cost[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector3>();
    }

    private List<Vector3> ReconstructPath(Dictionary<StreetNode, StreetNode> cameFrom, StreetNode current)
    {
        var path = new List<Vector3> { current.transform.position };

        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Insert(0, current.transform.position);
        }

        return path;
    }

    private StreetNode FindClosestNode(Vector3 position)
    {
        StreetNode closest = null;
        float minDist = float.MaxValue;

        foreach (var node in streetNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    /// <summary>
    /// Change the path style via dropdown or code.
    /// </summary>
    public void SetStyle(int index)
    {
        currentStyle = (PathStyle)index;
    }

    private Material GetMaterialForStyle()
    {
        return currentStyle switch
        {
            PathStyle.Line => lineMaterial,
            PathStyle.Arrow => arrowMaterial,
            PathStyle.Glow => glowMaterial,
            _ => lineMaterial
        };
    }
}
