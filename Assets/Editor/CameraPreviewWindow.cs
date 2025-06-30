#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CameraPreviewWindow : EditorWindow
{
    private Camera previewCamera;
    private RenderTexture previewTexture;

    private FocusAriaInteractable currentHotspot;
    private SmartCameraFocus focusRef;

    private static readonly Vector2 previewSize = new Vector2(512, 288); // 16:9

    private Transform pivotTransform;
    private bool livePreview = true;

    [MenuItem("Tools/Hotspot Camera Preview")]
    public static void ShowWindow()
    {
        GetWindow<CameraPreviewWindow>("Camera Preview");
    }

    private void OnEnable()
    {
        CreatePreviewCamera();
        EditorApplication.update += UpdatePreview;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdatePreview;
        DestroyPreviewCamera();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hotspot Camera Preview", EditorStyles.boldLabel);

        livePreview = EditorGUILayout.Toggle("Live Preview", livePreview);

        if (currentHotspot == null || focusRef == null)
        {
            EditorGUILayout.HelpBox("Select a valid hotspot and assign SmartCameraFocus to preview.", MessageType.Info);
            return;
        }

        if (previewTexture == null)
        {
            CreatePreviewTexture();
        }

        if (previewCamera != null)
        {
            previewCamera.Render();

            GUILayout.Box(previewTexture, GUILayout.Width(previewSize.x), GUILayout.Height(previewSize.y));
        }

        if (GUILayout.Button("📝 Save Pivot & Scene"))
        {
            EditorUtility.SetDirty(currentHotspot);
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(currentHotspot.gameObject.scene);
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("Scene and hotspot saved.");
            }
        }
    }

    private void CreatePreviewCamera()
    {
        GameObject camObj = new GameObject("PreviewCamera");
        camObj.hideFlags = HideFlags.HideAndDontSave;
        previewCamera = camObj.AddComponent<Camera>();
        previewCamera.enabled = false;
        previewCamera.fieldOfView = 60f;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = Color.gray;
        CreatePreviewTexture();
    }

    private void CreatePreviewTexture()
    {
        if (previewTexture == null)
        {
            previewTexture = new RenderTexture((int)previewSize.x, (int)previewSize.y, 24, RenderTextureFormat.ARGB32);
            previewTexture.Create();
        }

        if (previewCamera != null)
        {
            previewCamera.targetTexture = previewTexture;
        }
    }


    private void DestroyPreviewCamera()
    {
        if (previewCamera != null)
            DestroyImmediate(previewCamera.gameObject);

        if (previewTexture != null)
            previewTexture.Release();
    }

    public void SetPreview(FocusAriaInteractable hotspot, SmartCameraFocus focus, Vector3 offset, Quaternion rotation, Transform pivot)
    {
        currentHotspot = hotspot;
        focusRef = focus;
        pivotTransform = pivot;

        UpdateCameraPosition(); // Initial update
    }

    private void UpdatePreview()
    {
        if (!livePreview || currentHotspot == null || focusRef == null || pivotTransform == null || previewCamera == null)
            return;

        UpdateCameraPosition();
        Repaint();
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPos = pivotTransform.position;
        Vector3 cameraPos = targetPos + focusRef.viewOffset;
        Quaternion rotation = pivotTransform.localRotation;

        previewCamera.transform.position = cameraPos;
        previewCamera.transform.rotation = rotation;
    }
}
#endif
