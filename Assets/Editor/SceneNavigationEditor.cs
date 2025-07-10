using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SceneNavigationEditor : EditorWindow
{
    private float linkMaxDistance = 10f;
    private LayerMask nodeLayerMask = ~0;
    private GameObject parentRoadContainer;
    public GameObject pinPrefab; // Assign this via Inspector

    private string navPointName = "New Point";
    private EmergencyType selectedType = EmergencyType.FireStation;
    [MenuItem("Tools/Navigation/Scene Navigation Editor")]
    public static void Open()
    {
        GetWindow<SceneNavigationEditor>("Navigation Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Street Node Auto-Linking", EditorStyles.boldLabel);
        linkMaxDistance = EditorGUILayout.FloatField("Link Max Distance", linkMaxDistance);

        if (GUILayout.Button("Auto-Link All Nodes"))
        {
            AutoLinkAllNodes();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Attach Navigation Pins", EditorStyles.boldLabel);

        pinPrefab = (GameObject)EditorGUILayout.ObjectField("Pin Prefab", pinPrefab, typeof(GameObject), false);
        navPointName = EditorGUILayout.TextField("Point Name", navPointName);
        selectedType = (EmergencyType)EditorGUILayout.EnumPopup("Point Type", selectedType);

        if (GUILayout.Button("Attach Navigation Points to Selected"))
        {
            AttachPinsToSelected();
        }

        EditorGUILayout.HelpBox("To use: select GameObjects and click above to assign pin buttons.", MessageType.Info);
        EditorGUILayout.Space();

        GUILayout.Label("Generate Street Nodes", EditorStyles.boldLabel);

        parentRoadContainer = (GameObject)EditorGUILayout.ObjectField("Roads Parent", parentRoadContainer, typeof(GameObject), true);
        linkMaxDistance = EditorGUILayout.FloatField("Node Spacing (m)", linkMaxDistance);

        if (GUILayout.Button("Generate Nodes On Roads"))
        {
            if (parentRoadContainer == null)
            {
                Debug.LogWarning("Please assign a roads parent object.");
                return;
            }

            GenerateNodes();
        }


        void GenerateNodes()
        {
            int nodeCount = 0;

            foreach (Transform road in parentRoadContainer.transform)
            {
                MeshFilter meshFilter = road.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null)
                {
                    Debug.LogWarning($"Skipped {road.name} - no mesh.");
                    continue;
                }

                Bounds bounds = meshFilter.sharedMesh.bounds;
                Vector3 localStart = bounds.min;
                Vector3 localEnd = bounds.max;

                float length = Vector3.Distance(localStart, localEnd);
                int steps = Mathf.FloorToInt(length / linkMaxDistance);

                for (int i = 0; i <= steps; i++)
                {
                    float t = i / (float)steps;
                    Vector3 localPos = Vector3.Lerp(localStart, localEnd, t);
                    Vector3 worldPos = road.TransformPoint(localPos);

                    GameObject node = new GameObject($"Node_{i}");
                    node.transform.position = worldPos;
                    node.transform.SetParent(road);

                    StreetNode sn = node.AddComponent<StreetNode>();
                    sn.neighbors = new System.Collections.Generic.List<StreetNode>();

                    nodeCount++;
                }
            }

            Debug.Log($"[Node Generator] Generated {nodeCount} nodes.");
        }
    }

    private void AutoLinkAllNodes()
    {
        int totalLinks = 0;

        // Step 1: connect nodes within each road
        foreach (Transform road in parentRoadContainer.transform)
        {
            List<StreetNode> roadNodes = new();

            foreach (Transform child in road)
            {
                StreetNode sn = child.GetComponent<StreetNode>();
                if (sn != null)
                {
                    sn.neighbors = new List<StreetNode>();
                    roadNodes.Add(sn);
                }
            }

            for (int i = 0; i < roadNodes.Count - 1; i++)
            {
                roadNodes[i].neighbors.Add(roadNodes[i + 1]);
                roadNodes[i + 1].neighbors.Add(roadNodes[i]);
                totalLinks += 2;
            }
        }

        // Step 2: connect nearest nodes across roads as intersections
        var allRoads = parentRoadContainer.transform;
        List<StreetNode> allNodes = new();

        foreach (Transform r in allRoads)
        {
            foreach (Transform child in r)
            {
                StreetNode sn = child.GetComponent<StreetNode>();
                if (sn != null)
                    allNodes.Add(sn);
            }
        }

        foreach (Transform road in allRoads)
        {
            List<StreetNode> currentRoadNodes = new();
            foreach (Transform child in road)
            {
                StreetNode sn = child.GetComponent<StreetNode>();
                if (sn != null)
                    currentRoadNodes.Add(sn);
            }

            if (currentRoadNodes.Count == 0)
                continue;

            StreetNode start = currentRoadNodes[0];
            StreetNode end = currentRoadNodes[^1];

            StreetNode nearestToStart = FindClosestNodeOutsideRoad(start, allNodes, currentRoadNodes);
            StreetNode nearestToEnd = FindClosestNodeOutsideRoad(end, allNodes, currentRoadNodes);

            if (nearestToStart != null && !start.neighbors.Contains(nearestToStart))
            {
                start.neighbors.Add(nearestToStart);
                nearestToStart.neighbors.Add(start);
                totalLinks += 2;
            }

            if (nearestToEnd != null && !end.neighbors.Contains(nearestToEnd))
            {
                end.neighbors.Add(nearestToEnd);
                nearestToEnd.neighbors.Add(end);
                totalLinks += 2;
            }
        }

        Debug.Log($"[SceneNavigationEditor] Linked {totalLinks} node connections.");
        EditorUtility.SetDirty(this);
    }

    private StreetNode FindClosestNodeOutsideRoad(StreetNode source, List<StreetNode> all, List<StreetNode> exclude)
    {
        float minDist = float.MaxValue;
        StreetNode closest = null;

        foreach (StreetNode node in all)
        {
            if (exclude.Contains(node)) continue;
            float dist = Vector3.Distance(source.transform.position, node.transform.position);
            if (dist < linkMaxDistance && dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    private void AttachPinsToSelected()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("[SceneNavigationEditor] No GameObjects selected.");
            return;
        }

        foreach (GameObject go in selected)
        {
          

            NavigationPoint nav = go.GetComponent<NavigationPoint>();
            if (!nav  )
            {
                nav = go.AddComponent<NavigationPoint>();
                nav.name = navPointName;
                nav.type = selectedType;
                Debug.Log($"[SceneNavigationEditor] Added NavigationPoint to {go.name}");
            }

            // Create and attach pin prefab
            if (pinPrefab && nav.visualPin == null)
            {
                GameObject pin = (GameObject)PrefabUtility.InstantiatePrefab(pinPrefab);
                pin.name = $"Pin_{navPointName}";
                pin.transform.SetParent(go.transform);
                pin.transform.localPosition = new Vector3(0, 0f, 20); // 3m above node
                pin.transform.localRotation = pinPrefab.transform.localRotation;
                nav.visualPin = pin;
                pin.GetComponent<Canvas>().worldCamera = Camera.main;
                // Try to assign label if available
                TextMeshProUGUI label = pin.GetComponentInChildren<PinButton>().label ;
                if (label) label.text = navPointName;

                Debug.Log($"[SceneNavigationEditor] Created visual pin for {go.name}");
            }
        }

        EditorUtility.SetDirty(this);
    }
}
