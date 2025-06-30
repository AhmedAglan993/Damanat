using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle hoverToggle;
    public Toggle hotspotToggle;
    public Toggle autoFocusToggle;
    public Toggle soundToggle;
    public Toggle criticalOnlyToggle;

    [Header("Dropdowns")]
    public TMP_Dropdown floorDropdown;
    public TMP_Dropdown themeDropdown;
    public TMP_Dropdown cameraDropdown;
    public TMP_Dropdown navigationDropdown;

    void Start()
    {
        var prefs = SettingsManager.Instance.Preferences;

        hoverToggle.isOn = prefs.useHoverToOpenMenu;
        hotspotToggle.isOn = prefs.enableHotspotsByDefault;
        autoFocusToggle.isOn = prefs.autoFocusOnSelection;
        soundToggle.isOn = prefs.playSoundOnAlerts;
        criticalOnlyToggle.isOn = prefs.filterCriticalAlertsOnly;

        floorDropdown.SetValueWithoutNotify(floorDropdown.options.FindIndex(o => o.text == prefs.defaultFloor));
        themeDropdown.value = (int)prefs.theme;
        cameraDropdown.value = (int)prefs.defaultCameraAngle;
        navigationDropdown.value = (int)prefs.navigationMode;
    }

    public void OnHoverToggleChanged(bool value) => SettingsManager.Instance.Preferences.useHoverToOpenMenu = value;
    public void OnHotspotToggleChanged(bool value) => SettingsManager.Instance.Preferences.enableHotspotsByDefault = value;
    public void OnAutoFocusToggleChanged(bool value) => SettingsManager.Instance.Preferences.autoFocusOnSelection = value;
    public void OnSoundToggleChanged(bool value) => SettingsManager.Instance.Preferences.playSoundOnAlerts = value;
    public void OnCriticalToggleChanged(bool value) => SettingsManager.Instance.Preferences.filterCriticalAlertsOnly = value;

    public void OnFloorChanged(int index) =>
        SettingsManager.Instance.Preferences.defaultFloor = floorDropdown.options[index].text;
    public void OnThemeChanged(int index)
    {
        SettingsManager.Instance.Preferences.theme = (ThemeMode)index;
        var theme = (ThemeMode)index;
        SettingsManager.Instance.Preferences.theme = theme;
      //  ThemeManager.Instance.ApplyTheme(theme);
    }
    public void OnCameraChanged(int index)
    {
        SettingsManager.Instance.Preferences.defaultCameraAngle = (CameraAngle)index;
       
    }
    public void OnNavigationChanged(int index) =>
        SettingsManager.Instance.Preferences.navigationMode = (MovementType)index;
  

    public void OnSaveClicked() => SettingsManager.Instance.SavePreferences();
}
