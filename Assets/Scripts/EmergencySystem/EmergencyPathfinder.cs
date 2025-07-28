using System.Collections.Generic;
using UnityEngine;

public class EmergencyPathfinder : MonoBehaviour
{
    public static EmergencyPathfinder Instance;

    private List<EmergencyNode> allNodes;

    private void Awake()
    {
        Instance = this;
        allNodes = new List<EmergencyNode>(FindObjectsOfType<EmergencyNode>());
    }

    public List<EmergencyNode> GetPath(Transform startTransform)
    {
        EmergencyNode start = FindClosestNode(startTransform.position);
        EmergencyNode end = FindClosestExitNode();

        if (start == null || end == null)
        {
            Debug.LogWarning("Missing start or end node.");
            return new();
        }

        return Dijkstra(start, end);
    }

    EmergencyNode FindClosestNode(Vector3 pos)
    {
        float minDist = float.MaxValue;
        EmergencyNode closest = null;

        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(pos, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }
        return closest;
    }

    EmergencyNode FindClosestExitNode()
    {
        EmergencyNode best = null;
        float minDist = float.MaxValue;

        foreach (var node in allNodes)
        {
            if (!node.isExitNode) continue;

            float dist = Vector3.Distance(Camera.main.transform.position, node.transform.position);
            if (dist < minDist)
            {
                best = node;
                minDist = dist;
            }
        }

        return best;
    }

    List<EmergencyNode> Dijkstra(EmergencyNode start, EmergencyNode goal)
    {
        var frontier = new List<EmergencyNode> { start };
        var cameFrom = new Dictionary<EmergencyNode, EmergencyNode>();
        var costSoFar = new Dictionary<EmergencyNode, float> { [start] = 0f };

        while (frontier.Count > 0)
        {
            EmergencyNode current = frontier[0];

            foreach (var node in frontier)
                if (costSoFar[node] < costSoFar[current])
                    current = node;

            if (current == goal)
                break;

            frontier.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                float newCost = costSoFar[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    if (!frontier.Contains(neighbor))
                        frontier.Add(neighbor);
                }
            }
        }

        return ReconstructPath(cameFrom, goal);
    }

    List<EmergencyNode> ReconstructPath(Dictionary<EmergencyNode, EmergencyNode> cameFrom, EmergencyNode end)
    {
        var path = new List<EmergencyNode>();
        EmergencyNode current = end;
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current);
            current = cameFrom[current];
        }
        path.Insert(0, current);
        return path;
    }
}
