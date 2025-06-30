using Ricimi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotSpot : MonoBehaviour
{
    [SerializeField]public string hotSpotName;
    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = hotSpotName;
        GetComponentInChildren<CleanButton>().onClick.AddListener(() =>
        {
            GetComponentInChildren<StatisticsPopupOpener>().OpenPopup();
            print(GetComponent<HotSpot>().hotSpotName);
            RoomStatistics roomStatistics = new RoomStatistics(GetComponent<HotSpot>().hotSpotName);

           GetComponentInChildren<StatisticsPopupOpener>().popup.GetComponent<RoomStatisticsUIController>().ShowStatistics(roomStatistics);
        });
    }
    [ContextMenu("SetHotspotNames")]
    public void SetHotspotNames()
    {
        gameObject.name =  hotSpotName;
    }

}
