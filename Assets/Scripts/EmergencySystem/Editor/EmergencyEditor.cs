using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EmergencyEditor : EditorWindow
{
    private GameObject floorParent;
    private float nodeSpacing = 3f;
    private float linkDistance = 4f;
    private ExitType exitType = ExitType.GroundExit;

    [MenuItem("Tools/Emergency System/Emergency Editor")]
    public static void ShowWindow()
    {
        GetWindow<EmergencyEditor>("Emergency Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Emergency Exit Node Tool", EditorStyles.boldLabel);

        floorParent = (GameObject)EditorGUILayout.ObjectField("Floor Mesh Parent", floorParent, typeof(GameObject), true);
        nodeSpacing = EditorGUILayout.FloatField("Node Spacing", nodeSpacing);
        linkDistance = EditorGUILayout.FloatField("Link Max Distance", linkDistance);
        exitType = (ExitType)EditorGUILayout.EnumPopup("Default Exit Type", exitType);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Nodes on Floor"))
        {
            GenerateNodes();
        }

        if (GUILayout.Button("Auto-Link Nearby Nodes"))
        {
            AutoLinkNodes();
        }

        if (GUILayout.Button("Mark Selected as Exit Nodes"))
        {
            MarkAsExit();
        }

        if (GUILayout.Button("Clear All Emergency Nodes"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Delete all EmergencyNode components?", "Yes", "No"))
                ClearAllNodes();
        }
    }

    private void GenerateNodes()
    {
        if (floorParent == null)
        {
            Debug.LogWarning("Please assign a floor mesh parent.");
            return;
        }

        int count = 0;

        foreach (Transform floor in floorParent.transform)
        {
            MeshFilter meshFilter = floor.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogWarning($"Skipped {floor.name} - No mesh found.");
                continue;
            }

            Bounds bounds = meshFilter.sharedMesh.bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            Vector3 size = bounds.size;

            for (float x = 0; x <= size.x; x += nodeSpacing)
            {
                for (float z = 0; z <= size.z; z += nodeSpacing)
                {
                    Vector3 localPos = new Vector3(min.x + x, 0, min.z + z);
                    Vector3 worldPos = floor.TransformPoint(localPos);

                    GameObject nodeObj = new GameObject($"Node_{count}");
                    nodeObj.transform.position = worldPos;
                    nodeObj.transform.SetParent(floor);

                    EmergencyNode node = nodeObj.AddComponent<EmergencyNode>();
                    node.isExitNode = false;
                    node.neighbors = new List<EmergencyNode>();

                    count++;
                }
            }
        }

        Debug.Log($"[EmergencyEditor] Generated {count} nodes.");
    }

    private void AutoLinkNodes()
    {
        EmergencyNode[] nodes = FindObjectsOfType<EmergencyNode>();
        int totalLinks = 0;

        foreach (var node in nodes)
        {
            node.neighbors.Clear();

            foreach (var other in nodes)
            {
                if (node == other) continue;

                float dist = Vector3.Distance(node.transform.position, other.transform.position);
                if (dist <= linkDistance)
                    node.neighbors.Add(other);
            }

            totalLinks += node.neighbors.Count;
        }

        Debug.Log($"[EmergencyEditor] Linked {nodes.Length} nodes with {totalLinks} total neighbor connections.");
    }

    private void MarkAsExit()
    {
        foreach (var obj in Selection.gameObjects)
        {
            EmergencyNode node = obj.GetComponent<EmergencyNode>();
            if (node == null)
            {
                Debug.LogWarning($"{obj.name} is not an EmergencyNode.");
                continue;
            }

            node.isExitNode = true;
            node.exitType = exitType;

            EmergencyExit exit = obj.GetComponent<EmergencyExit>();
            if (exit == null)
                exit = obj.AddComponent<EmergencyExit>();

            exit.exitType = exitType;
            exit.linkedNode = node;

            Debug.Log($"Marked {obj.name} as exit node ({exitType})");
        }
    }

    private void ClearAllNodes()
    {
        EmergencyNode[] nodes = FindObjectsOfType<EmergencyNode>();
        int removed = 0;

        foreach (var node in nodes)
        {
            DestroyImmediate(node.gameObject);
            removed++;
        }

        Debug.Log($"[EmergencyEditor] Removed {removed} emergency nodes.");
    }
}
