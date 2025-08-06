using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenAlertManager : AlertPopupManager
{
    public void Evacuate()
    {
        EmergencyManager.Instance.EvacuateAll();
        HologramSwitcher.Instance.RevealHologram();

        ClosePopup();
    }
    public override void ShowAlert(AlertEntry alertEntry, Color color)
    {
        popupRoot.SetActive(true);

        titleText.text = $"{alertEntry.alertTitle}";
        timeText.text = $"<b>Time:</b> {alertEntry.alertTime}";
        locationText.text = $"<b>Location:</b> {alertEntry.alertLocation}";
        descriptionText.text = $"<b>Cause:</b>{alertEntry.alertDescription}";
        actionText.text = $"<b>Suggested Action:</b> {alertEntry.alertAction}";
    }
}
