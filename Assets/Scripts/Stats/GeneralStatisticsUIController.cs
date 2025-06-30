using UnityEngine;
using TMPro;

public class GeneralStatisticsUIController : BaseStatisticsUIController
{
    [Header("General Statistics UI Cards")]
    [SerializeField] TMP_Text trafficCardText;
    [SerializeField] TMP_Text vehiclesCardText;
    [SerializeField] TMP_Text peopleCardText;
    [SerializeField] TMP_Text lastUpdateCardText;

    protected override void ShowGeneralStatistics(GeneralStatistics stats)
    {
        trafficCardText.text = $"{stats.averageTraffic:F0} /hr";
        vehiclesCardText.text = $"{stats.totalVehicles}";
        peopleCardText.text = $"{stats.totalPeopleInArea}";
        lastUpdateCardText.text = stats.lastUpdate;
    }

    protected override void ShowRoomStatistics(RoomStatistics stats)
    {
        // Optional: Do nothing or log
    }
}
