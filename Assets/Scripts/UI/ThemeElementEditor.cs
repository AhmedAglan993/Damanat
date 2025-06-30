#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ThemeElement))]
public class ThemeElementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ThemeElement theme = (ThemeElement)target;

        if (GUILayout.Button("Set Light Color From Current"))
        {
            if (theme.TryGetComponent(out Graphic g))
            {
                theme.lightColor = g.color;
                EditorUtility.SetDirty(theme);
            }
        }

        if (GUILayout.Button("Set Dark Color From Current"))
        {
            if (theme.TryGetComponent(out Graphic g))
            {
                theme.darkColor = g.color;
                EditorUtility.SetDirty(theme);
            }
        }

        if (theme.applyMode == ThemeElement.ApplyMode.Gradient && GUILayout.Button("Set light Gradient From Current"))
        {
            if (theme.TryGetComponent(out Ricimi.Gradient gr))
            {
                theme.lightGradient = new GradientSettings
                {
                    color1 = gr.Color1,
                    color2 = gr.Color2,
                    angle = gr.Angle
                };
                EditorUtility.SetDirty(theme);
            }
        }
        if (theme.applyMode == ThemeElement.ApplyMode.Gradient && GUILayout.Button("Set Dark Gradient From Current"))
        {
            if (theme.TryGetComponent(out Ricimi.Gradient gr))
            {
                theme.darkGradient = new GradientSettings
                {
                    color1 = gr.Color1,
                    color2 = gr.Color2,
                    angle = gr.Angle
                };
                EditorUtility.SetDirty(theme);
            }
        }
    }
}
#endif
