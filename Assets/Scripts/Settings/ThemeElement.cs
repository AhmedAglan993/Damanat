using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Graphic))]
public class ThemeElement : MonoBehaviour
{
    public enum ApplyMode { Color, Gradient }
    public ApplyMode applyMode = ApplyMode.Color;

    [Header("Colors")]
    public Color lightColor = Color.white;
    public Color darkColor = Color.black;

    [Header("Gradient (if used)")]
    public GradientSettings lightGradient;
    public GradientSettings darkGradient;

    private Graphic graphic;
    private Ricimi.Gradient gradient;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
        gradient = GetComponent<Ricimi.Gradient>();
    }

    public void ApplyTheme(bool useDarkMode)
    {
        if (applyMode == ApplyMode.Color && graphic != null)
        {
            graphic.color = useDarkMode ? darkColor : lightColor;
        }

        if (applyMode == ApplyMode.Gradient && gradient != null)
        {
            var g = useDarkMode ? darkGradient : lightGradient;
            gradient.Color1 = g.color1;
            gradient.Color2 = g.color2;
            gradient.Angle = g.angle;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(gradient);
#endif
        }
    }
}

[System.Serializable]
public struct GradientSettings
{
    public Color color1;
    public Color color2;
    [Range(-180f, 180f)] public float angle;
}
