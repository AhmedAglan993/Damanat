using UnityEngine;

[CreateAssetMenu(menuName = "DigitalTwin/User Preferences", fileName = "UserPreferencesData")]
public class UserPreferencesData : ScriptableObject
{
    public UserPreferences currentPreferences = new UserPreferences();
}
