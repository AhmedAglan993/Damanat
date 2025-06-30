using UnityEngine;
using UIGradient = Ricimi.Gradient;
public class ThemeManager : MonoBehaviour
{
    public static bool IsDarkMode { get; private set; }

    private void Start()
    {
        SetDarkMode(false); // start with light mode (or set to true if you want dark)
    }

    public void SetDarkMode(bool dark)
    {
        IsDarkMode = dark;

        foreach (var element in FindObjectsOfType<ThemeElement>(true))
        {
            element.ApplyTheme(dark);
            if (element.GetComponent<UIGradient>())
            {
                element.GetComponent<UIGradient>().enabled = false;
                element.GetComponent<UIGradient>().enabled = true;
            }
          
        }
    }

    public void ToggleTheme()
    {
        SetDarkMode(!IsDarkMode);
    }
}
