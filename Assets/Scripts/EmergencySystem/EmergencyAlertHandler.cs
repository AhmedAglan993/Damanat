using UnityEngine;

public class EmergencyAlertHandler : MonoBehaviour
{
    public static EmergencyAlertHandler Instance;

    public TimelineUIController timelineController;
    public AudioSource alarmAudio;
    public OnScreenAlertManager onScreenAlertManager;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerEmergency(AlertEntry entry, Transform location, int floor)
    {
        string time = System.DateTime.Now.ToString("HH:mm");
        AlertDatabase.SaveAlert(entry);
        timelineController.GenerateBaseTimeline();
        onScreenAlertManager.ShowAlert(entry, Color.red);
        // FloorsManager.Instance.SelectFloorToShow(floor);
        EmergencyManager.Instance.emergencyArea = location;
        if (alarmAudio && !alarmAudio.isPlaying)
            alarmAudio.Play();
    }
}
