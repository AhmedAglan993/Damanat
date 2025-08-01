using UnityEngine;

public class EmergencyAlertHandler : MonoBehaviour
{
    public static EmergencyAlertHandler Instance;

    public TimelineUIController timelineController;
    public EmergencyManager emergencyManager;
    public AudioSource alarmAudio;
    public GameObject onScreenEmergencyPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerEmergency(AlertEntry entry, Transform location, int floor)
    {
        string time = System.DateTime.Now.ToString("HH:mm");

        // 1. Create AlertEntry
       

        // 2. Add to timeline
        AlertDatabase.SaveAlerts(timelineController.alertEntries);
        timelineController.GenerateBaseTimeline(); // Or a method to add one alert more efficiently

        // 3. Show alert panel
        onScreenEmergencyPanel.SetActive(true);

        // 4. Play alarm sound
        if (alarmAudio && !alarmAudio.isPlaying)
            alarmAudio.Play();

        // 5. Show exit path for this floor
        emergencyManager.ShowExitPathFrom(location);
    }
}
