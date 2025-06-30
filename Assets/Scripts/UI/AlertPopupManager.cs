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

    public void ShowAlert(AlertEntry alertEntry)
    {
        popupRoot.SetActive(true);

        titleText.text = $"Alert: {alertEntry.alertTitle}";
        timeText.text = $"Time: {alertEntry.alertTime}";
        locationText.text = $"Location: {alertEntry.alertLocation}";
        descriptionText.text = alertEntry.alertDescription;
        actionText.text = $"Suggested Action: {alertEntry.alertAction}";
    }

    public void ClosePopup()
    {
        popupRoot.SetActive(false);
    }
}
