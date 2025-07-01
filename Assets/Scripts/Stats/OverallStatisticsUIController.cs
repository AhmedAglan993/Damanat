using Ricimi;
using UnityEngine;

public class OverallStatisticsUIController : MonoBehaviour
{
    public static OverallStatisticsUIController Instance;

    [Header("Statistics Popups")]
    [SerializeField] private GameObject generalStatsPopup;
    [SerializeField] private GameObject perRoomStatsPopup;

    private bool isInRoom = false;

    private void Awake()
    {
        Instance = this;
        GetComponent<CleanButton>().onClick.AddListener(OpenStatistics);
    }

    public void SetIsInRoom(bool value)
    {
        isInRoom = value;
    }

    public bool IsInRoom()
    {
        return isInRoom;
    }

    public void OpenStatistics()
    {
        var popupOpener = GetComponent<PopupOpener>();
        popupOpener.popup = isInRoom ? perRoomStatsPopup : generalStatsPopup;
        popupOpener.OpenPopup();
    }
}
