using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class MasterPlanHotspotEditor : EditorWindow
{
    public GameObject floorModel;
    public GameObject hotspotPrefab;
    public GameObject hotspotsParent;
    public Camera captureCamera;
    private Texture2D masterPlanImage;
    private Vector2 innerScroll; // Moved from inside OnGUI to here

    [System.Serializable]
    public class HotspotData
    {
        public string name;
        public Vector2 normalizedPosition;
    }

    [System.Serializable]
    public class HotspotListWrapper
    {
        public List<HotspotData> hotspots;
    }

    public List<HotspotData> masterPlanData = new List<HotspotData>();

    public Vector3 floorCornerTopLeft = new Vector3(-5, 0, 5);
    public Vector3 floorCornerTopRight = new Vector3(5, 0, 5);
    public Vector3 floorCornerBottomLeft = new Vector3(-5, 0, -5);
    public Vector3 floorCornerBottomRight = new Vector3(5, 0, -5);

    private Vector2 scrollPos;
    private float zoom = 1f;
    private Vector2 zoomScrollPos;

    [MenuItem("Tools/Master Plan Hotspot Editor")]
    public static void ShowWindow()
    {
        MasterPlanHotspotEditor window = GetWindow<MasterPlanHotspotEditor>("Master Plan Hotspot Editor");
        window.minSize = new Vector2(400, 600);
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Master Plan Hotspot Editor", EditorStyles.boldLabel);
        GUILayout.Space(10);

        floorModel = (GameObject)EditorGUILayout.ObjectField("Floor Model", floorModel, typeof(GameObject), true);
        hotspotPrefab = (GameObject)EditorGUILayout.ObjectField("Hotspot Prefab", hotspotPrefab, typeof(GameObject), false);
        hotspotsParent = (GameObject)EditorGUILayout.ObjectField("Hotspot Parent", hotspotsParent, typeof(GameObject), true);
        captureCamera = (Camera)EditorGUILayout.ObjectField("Capture Camera", captureCamera, typeof(Camera), true);

        if (GUILayout.Button("Capture Plan Image From Camera") && captureCamera != null)
        {
            masterPlanImage = CaptureCameraImage();
        }

        if (masterPlanImage != null)
        {
            GUILayout.Label("Master Plan Image (Click to add hotspot):");

            zoom = GUILayout.HorizontalSlider(zoom, 0.1f, 3f);
            GUILayout.Label($"Zoom: {zoom:F2}x");

            float imageWidth = 1080 * zoom;
            float imageHeight = 1920 * zoom;

            zoomScrollPos = EditorGUILayout.BeginScrollView(zoomScrollPos, GUILayout.Height(500));
            Rect imageRect = GUILayoutUtility.GetRect(imageWidth, imageHeight, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            EditorGUI.DrawPreviewTexture(imageRect, masterPlanImage);

            if (Event.current.type == EventType.MouseDown && imageRect.Contains(Event.current.mousePosition))
            {
                Vector2 clickPos = Event.current.mousePosition - new Vector2(imageRect.x, imageRect.y);
                Vector2 normalizedPos = new Vector2(clickPos.x / imageRect.width, 1f - clickPos.y / imageRect.height);
                AddHotspot(normalizedPos);
                Event.current.Use();
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(10);

        GUILayout.Label("Floor Corners", EditorStyles.boldLabel);
        floorCornerTopLeft = EditorGUILayout.Vector3Field("Top Left", floorCornerTopLeft);
        floorCornerTopRight = EditorGUILayout.Vector3Field("Top Right", floorCornerTopRight);
        floorCornerBottomLeft = EditorGUILayout.Vector3Field("Bottom Left", floorCornerBottomLeft);
        floorCornerBottomRight = EditorGUILayout.Vector3Field("Bottom Right", floorCornerBottomRight);

        GUILayout.Space(10);

        GUILayout.Label("Hotspots List:", EditorStyles.boldLabel);
        innerScroll = EditorGUILayout.BeginScrollView(innerScroll, GUILayout.Height(150));

        for (int i = 0; i < masterPlanData.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            masterPlanData[i].name = EditorGUILayout.TextField(masterPlanData[i].name);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                masterPlanData.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Clear Hotspots"))
        {
            masterPlanData.Clear();
            foreach (Transform child in floorModel.transform)
            {
                if (child.name.StartsWith("Hotspot "))
                    Undo.DestroyObjectImmediate(child.gameObject);
            }
        }

        if (GUILayout.Button("Apply Hotspots To Floor"))
            ApplyHotspotsToFloor();

        GUILayout.Space(10);
        GUILayout.Label("Save/Load Hotspots", EditorStyles.boldLabel);

        if (GUILayout.Button("Save Hotspots to JSON"))
        {
            string path = EditorUtility.SaveFilePanel("Save Hotspots", "", "hotspots.json", "json");
            if (!string.IsNullOrEmpty(path)) SaveHotspotsToJson(path);
        }

        if (GUILayout.Button("Load Hotspots from JSON"))
        {
            string path = EditorUtility.OpenFilePanel("Load Hotspots", "", "json");
            if (!string.IsNullOrEmpty(path)) LoadHotspotsFromJson(path);
        }

        EditorGUILayout.EndScrollView();
    }

    private Texture2D CaptureCameraImage()
    {
        int width = 1080;
        int height = 1920;
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        captureCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        return screenshot;
    }

    private void AddHotspot(Vector2 normalizedPos)
    {
        string newName = "Hotspot " + (masterPlanData.Count + 1);
        var hotspot = new HotspotData() { name = newName, normalizedPosition = normalizedPos };
        masterPlanData.Add(hotspot);

        if (hotspotPrefab != null && floorModel != null)
        {
            Vector3 approxPos = GetWorldPositionFromNormalized(normalizedPos) + Vector3.up * 10f;
            if (Physics.Raycast(approxPos, Vector3.down, out RaycastHit hit, 20f))
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(hotspotPrefab);
                Undo.RegisterCreatedObjectUndo(instance, "Place Hotspot");
                instance.name = newName;
                instance.transform.position = hit.point;
                instance.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, hit.normal), hit.normal);
                instance.transform.SetParent(floorModel.transform);
            }
        }
    }

    private Vector3 GetWorldPositionFromNormalized(Vector2 normPos)
    {
        Vector3 topInterp = Vector3.Lerp(floorCornerTopLeft, floorCornerTopRight, normPos.x);
        Vector3 bottomInterp = Vector3.Lerp(floorCornerBottomLeft, floorCornerBottomRight, normPos.x);
        return Vector3.Lerp(bottomInterp, topInterp, normPos.y);
    }

    private void ApplyHotspotsToFloor()
    {
        if (hotspotPrefab == null || floorModel == null)
        {
            Debug.LogError("Missing references: Hotspot Prefab or Floor Model");
        }

        foreach (Transform child in floorModel.transform)
        {
            if (child.name.StartsWith("Hotspot "))
                Undo.DestroyObjectImmediate(child.gameObject);
        }

        foreach (var hotspot in masterPlanData)
        {
            Vector3 approxPos = GetWorldPositionFromNormalized(hotspot.normalizedPosition) + Vector3.up * 5f;
            GameObject hotspotClone = (GameObject)PrefabUtility.InstantiatePrefab(hotspotPrefab, hotspotsParent.transform);
            Undo.RegisterCreatedObjectUndo(hotspotClone, "Place Hotspot");
            hotspotClone.name = hotspot.name;
            hotspotClone.transform.position = approxPos;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void SaveHotspotsToJson(string path)
    {
        HotspotListWrapper wrapper = new HotspotListWrapper { hotspots = masterPlanData };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("Hotspots saved to: " + path);
    }

    private void LoadHotspotsFromJson(string path)
    {
        string json = File.ReadAllText(path);
        HotspotListWrapper wrapper = JsonUtility.FromJson<HotspotListWrapper>(json);
        masterPlanData = wrapper.hotspots;
        Repaint();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        EditorGUI.BeginChangeCheck();

        Handles.color = Color.cyan;
        floorCornerTopLeft = Handles.PositionHandle(floorCornerTopLeft, Quaternion.identity);
        floorCornerTopRight = Handles.PositionHandle(floorCornerTopRight, Quaternion.identity);
        floorCornerBottomLeft = Handles.PositionHandle(floorCornerBottomLeft, Quaternion.identity);
        floorCornerBottomRight = Handles.PositionHandle(floorCornerBottomRight, Quaternion.identity);

        Handles.color = Color.green;
        Handles.DrawLine(floorCornerTopLeft, floorCornerTopRight);
        Handles.DrawLine(floorCornerTopRight, floorCornerBottomRight);
        Handles.DrawLine(floorCornerBottomRight, floorCornerBottomLeft);
        Handles.DrawLine(floorCornerBottomLeft, floorCornerTopLeft);

        Handles.color = Color.red;
        foreach (var hotspot in masterPlanData)
        {
            Vector3 pos = GetWorldPositionFromNormalized(hotspot.normalizedPosition);
            Handles.SphereHandleCap(0, pos + Vector3.up * 0.1f, Quaternion.identity, 0.15f, EventType.Repaint);
            Handles.Label(pos + Vector3.up * 0.3f, hotspot.name);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Repaint();
        }
    }
}
