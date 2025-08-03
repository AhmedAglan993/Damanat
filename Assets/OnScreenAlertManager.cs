using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenAlertManager : AlertPopupManager
{
    public void Evacuate()
    {
        EmergencyManager.Instance.EvacuateAll();
        ClosePopup();
    }
}
