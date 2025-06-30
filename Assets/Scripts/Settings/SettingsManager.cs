using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private UserPreferencesData preferencesData;

    public UserPreferences Preferences => preferencesData.currentPreferences;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPreferences(); // load from PlayerPrefs if available
    }

    public void SavePreferences()
    {
        string json = JsonUtility.ToJson(preferencesData.currentPreferences);
        PlayerPrefs.SetString("UserPreferences", json);
        PlayerPrefs.Save();
    }

    public void LoadPreferences()
    {
        if (PlayerPrefs.HasKey("UserPreferences"))
        {
            string json = PlayerPrefs.GetString("UserPreferences");
            preferencesData.currentPreferences = JsonUtility.FromJson<UserPreferences>(json);
        }
    }
}

