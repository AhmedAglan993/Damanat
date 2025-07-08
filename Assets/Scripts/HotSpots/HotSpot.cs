using Ricimi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotSpot : MonoBehaviour
{
    [SerializeField] public string hotSpotName;

    [Header("Optional")]
    [SerializeField] private bool hasCeiling = false;
    [SerializeField] private GameObject ceilingObject;

    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = hotSpotName;
        GetComponentInChildren<CleanButton>().onClick.AddListener(() =>
        {
            GetComponentInChildren<StatisticsPopupOpener>().OpenPopup();
            OverallStatisticsUIController.Instance.SetIsInRoom(true);
            RoomStatistics stats = new RoomStatistics(hotSpotName);
            GetComponentInChildren<StatisticsPopupOpener>().popup.GetComponent<RoomStatisticsUIController>().ShowStatistics(stats);
            foreach (HotSpot hotSpot in UIManager.Instance.CurrenthotSpotsHolder.HotSpots){
                hotSpot.ToggleCeiling(true);
            }
            // 🔻 Hide ceiling if needed
            if (hasCeiling && ceilingObject != null)
                ceilingObject.SetActive(false);
        });
    }

    [ContextMenu("SetHotspotNames")]
    public void SetHotspotNames()
    {
        gameObject.name = hotSpotName;
    }

    public void ToggleCeiling(bool enable)
    {
        if (hasCeiling && ceilingObject != null)
            ceilingObject.SetActive(enable);
    }
}

