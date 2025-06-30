using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatisticsUIController : MonoBehaviour
{
    [Header("Text Fields")]
    public TMP_Text roomNameText;
    public TMP_Text occupancyText;
    public TMP_Text energyText;
    public TMP_Text tempText;
    public TMP_Text airText;
    public TMP_Text visitorsText;
    public TMP_Text lastMaintText;

    [Header("Progress")]
    public Image occupancyRing;
    public Image tempImage;
    public Image EnergyImage; // fillAmount 0 to 1


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
