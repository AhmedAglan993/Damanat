using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatisticsUIController : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text occupancyText;
    [SerializeField] TMP_Text energyText;
    [SerializeField] TMP_Text tempText;
    [SerializeField] TMP_Text airText;
    [SerializeField] TMP_Text visitorsText;
    [SerializeField] TMP_Text lastMaintText;

    [Header("Progress")]
    public Image occupancyRing;
    public Image tempImage;
    public Image EnergyImage; // fillAmount 0 to 1
    [SerializeField] private TMP_Text trafficCardText;
    [SerializeField] private TMP_Text vehiclesCardText;
    [SerializeField] private TMP_Text peopleCardText;
    [SerializeField] private TMP_Text lastUpdateCardText;

    public void ShowGeneralStatisticsCards(GeneralStatistics stats)
    {
        if (stats == null) return;

        trafficCardText.text = $"{stats.averageTraffic:F0} /hr";
        vehiclesCardText.text = $"{stats.totalVehicles}";
        peopleCardText.text = $"{stats.totalPeopleInArea}";
        lastUpdateCardText.text = stats.lastUpdate;
    }


    public void ShowStatistics(RoomStatistics stats)
    {
        if (stats == null) return;

        roomNameText.text = stats.roomName;
        occupancyText.text = $"{stats.occupancy:F0}%";
        energyText.text = $"{stats.energyUsage:F1} kWh";
        AnimateProgress(stats.energyUsage / 100f, EnergyImage);
        tempText.text = $"{stats.temperature:F1} °C";
        AnimateProgress(stats.temperature / 100f, tempImage);
        airText.text = $"AQI {stats.airQuality:F0}";
        visitorsText.text = stats.visitors.ToString();
        lastMaintText.text = stats.lastMaintenance;
        AnimateProgress(stats.occupancy / 100f, occupancyRing);
    }

    private void AnimateProgress(float targetFill, Image progress)
    {
        ;
        float duration = 1f;
        progress.fillAmount = 0;
        progress.DOFillAmount(targetFill, duration).SetEase(Ease.Linear);
    }


}
