using System.Collections.Generic;
using UnityEngine;

public class EmergencyNode : MonoBehaviour
{
    public List<EmergencyNode> neighbors = new();
    public bool isExitNode,FloorConnector;
    public ExitType exitType;

    private void OnDrawGizmos()
    {
        Gizmos.color = isExitNode ? Color.green : Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.3f);

        if (neighbors != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var n in neighbors)
                if (n != null) Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
}
