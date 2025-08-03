using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class EmergencyEditor : EditorWindow
{
    private GameObject floorParent;
    private float nodeSpacing = 3f;
    private float linkDistance = 4f;
    private ExitType exitType = ExitType.GroundExit;
    private bool isPlacingNodes = false;
    private GameObject nodeParent;


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
        EditorGUILayout.Space();
        GUILayout.Label("Manual Placement Mode", EditorStyles.boldLabel);

        nodeParent = (GameObject)EditorGUILayout.ObjectField("Node Parent", nodeParent, typeof(GameObject), true);

        if (!isPlacingNodes && GUILayout.Button("Start Placing Nodes"))
        {
            isPlacingNodes = true;
            SceneView.duringSceneGui += OnSceneGUI;
            Debug.Log("[EmergencyEditor] Node placement mode ENABLED.");
        }

        if (isPlacingNodes && GUILayout.Button("Stop Placing Nodes"))
        {
            isPlacingNodes = false;
            SceneView.duringSceneGui -= OnSceneGUI;
            Debug.Log("[EmergencyEditor] Node placement mode DISABLED.");
        }

    }
    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 point = hit.point;

                GameObject nodeObj = new GameObject("ManualNode");
                nodeObj.transform.position = point;
                if (nodeParent) nodeObj.transform.SetParent(nodeParent.transform);

                EmergencyNode node = nodeObj.AddComponent<EmergencyNode>();
                node.isExitNode = false;
                node.neighbors = new List<EmergencyNode>();

                Debug.Log($"[EmergencyEditor] Node placed at {point}");

                e.Use(); // consume click
            }
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
        EmergencyNode[] allNodes = FindObjectsOfType<EmergencyNode>();
        int totalLinks = 0;

        Dictionary<Transform, List<EmergencyNode>> floorNodeMap = new();

        // Step 1: Group nodes by floor (assuming parent = floor GameObject)
        foreach (var node in allNodes)
        {
            Transform floor = node.transform.parent;
            if (!floorNodeMap.ContainsKey(floor))
                floorNodeMap[floor] = new List<EmergencyNode>();

            floorNodeMap[floor].Add(node);
        }

        // Step 2: Intra-floor linking
        foreach (var kvp in floorNodeMap)
        {
            List<EmergencyNode> nodes = kvp.Value;

            foreach (var node in nodes)
            {
                node.neighbors.Clear(); // reset

                foreach (var other in nodes)
                {
                    if (node == other) continue;

                    float dist = Vector3.Distance(node.transform.position, other.transform.position);
                    if (dist <= linkDistance)
                        node.neighbors.Add(other);
                }

                totalLinks += node.neighbors.Count;
            }
        }

        // Step 3: Inter-floor linking via connectors only
        List<EmergencyNode> connectors = new();
        foreach (var node in allNodes)
        {
            if (node.GetComponent<EmergencyNode>().FloorConnector)
                connectors.Add(node);
        }

        foreach (var node in connectors)
        {
            foreach (var other in connectors)
            {
                if (node == other) continue;

                float dist = Vector3.Distance(node.transform.position, other.transform.position);
                if (dist <= linkDistance && !node.neighbors.Contains(other))
                {
                    node.neighbors.Add(other);
                    totalLinks++;
                }
            }
        }

        Debug.Log($"[EmergencyEditor] Linked nodes with {totalLinks} total neighbor connections (intra + inter-floor).");
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
