using UnityEngine;

[ExecuteAlways]
public class StreetNodeGizmoDrawer : MonoBehaviour
{
    public Color nodeColor = Color.green;
    public Color connectionColor = Color.yellow;
    public float nodeSphereRadius = 0.3f;

    private void OnDrawGizmos()
    {
        var allNodes = FindObjectsOfType<StreetNode>();
        Gizmos.color = nodeColor;

        foreach (var node in allNodes)
        {
            if (node == null) continue;

            // Draw the node itself
            Gizmos.DrawSphere(node.transform.position, nodeSphereRadius);

            // Draw connections
            if (node.neighbors != null)
            {
                Gizmos.color = connectionColor;
                foreach (var neighbor in node.neighbors)
                {
                    if (neighbor != null)
                        Gizmos.DrawLine(node.transform.position, neighbor.transform.position);
                }
            }
        }
    }
}
