using UnityEngine;

public class EmergencyRoomTrigger : MonoBehaviour
{
    public string roomName => name;
    public int floorNumber => transform.parent.GetComponent<HotSpotsHolder>().floorNumber;
    public string alertType = "Fire";
    public string severity = "Critical";
    private void Start()
    {
       // TriggerAlarm();
        print("TriggerAlarm");
    }

    [ContextMenu("Trigger Emergency Alarm")]
    public void TriggerAlarm()
    {
        string time = System.DateTime.Now.ToString("HH:mm");

        AlertEntry entry = new AlertEntry
        {
            alertTitle = $"{alertType} in {roomName}",
            alertTime = time,
            alertLocation = $"{floorNumber} - {roomName}",
            alertDescription = $"Detected {alertType} in {roomName}.",
            alertAction = "Evacuate immediately.",
            alertType = alertType,
            alertSeverity = severity
        };

        EmergencyAlertHandler.Instance.TriggerEmergency(entry, transform, floorNumber);
    }
}
