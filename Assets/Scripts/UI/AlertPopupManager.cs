using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AlertPopupManager : MonoBehaviour
{
    public GameObject popupRoot;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI actionText;

    public void ShowAlert(AlertEntry alertEntry, Color color)
    {
        popupRoot.SetActive(true);

        titleText.text = $"Alert: {alertEntry.alertTitle}";
        titleText.color = color;
        timeText.text = $"<color=#C3BFBF><b>Time:</b></color> {alertEntry.alertTime}";
        locationText.text = $"<color=#C3BFBF><b>Location:</b></color> {alertEntry.alertLocation}";
        descriptionText.text = $"<color=#C3BFBF><b>Cause:</b></color> {alertEntry.alertDescription}";
        actionText.text = $"<color=#C3BFBF><b>Suggested Action:</b></color> {alertEntry.alertAction}";
    }

    public void ClosePopup()
    {
        popupRoot.SetActive(false);
    }
}
