#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FocusAriaInteractable))]
public class FocusAriaInteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FocusAriaInteractable hotspot = (FocusAriaInteractable)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Hotspot Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("📌 Create Pivot"))
        {
            Undo.RegisterCompleteObjectUndo(hotspot.gameObject, "Create View Pivot");
            hotspot.CreateAndAssignPivot();
        }
        if (GUILayout.Button("📌 Align Pivot from Rotation Preset"))
        {
            Undo.RegisterCompleteObjectUndo(hotspot.gameObject, "Align Pivot");
            hotspot.AlignPivot();
        }

        if (GUILayout.Button("▶ Preview Focus"))
        {
            var pivot = hotspot.GetViewPivot();
            if (pivot == null)
            {
                Debug.LogWarning("No view pivot assigned. Please create one.");
                return;
            }

            // 🔍 Snap Scene View to Pivot
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                sceneView.LookAt(pivot.position, pivot.rotation, 5f);
            }

            // ✅ Select the pivot GameObject in the Hierarchy
            Selection.activeGameObject = pivot.gameObject;

            // 📷 Open Preview Window and Setup
            CameraPreviewWindow window = (CameraPreviewWindow)EditorWindow.GetWindow(typeof(CameraPreviewWindow));
            window.Show();

            SmartCameraFocus focus = Camera.main?.GetComponent<SmartCameraFocus>();
            if (focus != null)
            {
                window.SetPreview(hotspot, focus, focus.viewOffset, pivot.localRotation, pivot);
            }
            else
            {
                Debug.LogWarning("SmartCameraFocus not found on the main camera.");
            }

            CameraPreviewWindow.ShowWindow();
        }


    }
}
#endif
