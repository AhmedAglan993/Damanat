using UnityEngine;

[CreateAssetMenu(menuName = "DigitalTwin/Theme Data")]
public class ThemeData : ScriptableObject
{
    public Color backgroundColor = Color.white;
    public Color textColor = Color.black;
    public Color iconTint = Color.gray;
}
