using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RoomStatisticsUIController : BaseStatisticsUIController
{
    [Header("Room Statistics UI")]
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text occupancyText;
    [SerializeField] TMP_Text energyText;
    [SerializeField] TMP_Text tempText;
    [SerializeField] TMP_Text lastMaintText;

    [Header("Progress Bars")]
    [SerializeField] Image occupancyRing;
    [SerializeField] Image tempImage;
    [SerializeField] Image energyImage;

    protected override void ShowRoomStatistics(RoomStatistics stats)
    {
        roomNameText.text = stats.roomName;
        occupancyText.text = $"{stats.occupancy:F0}%";
        energyText.text = $"{stats.energyUsage:F1} kWh";
        AnimateProgress(stats.energyUsage / 100f, energyImage);

        tempText.text = $"{stats.temperature:F1} °C";
        AnimateProgress(stats.temperature / 100f, tempImage);

        lastMaintText.text = stats.lastMaintenance;
        AnimateProgress(stats.occupancy / 100f, occupancyRing);
    }

    protected override void ShowGeneralStatistics(GeneralStatistics stats)
    {
        // Optional: Do nothing or log
    }

    private void AnimateProgress(float targetFill, Image progress)
    {
        float duration = 1f;
        progress.fillAmount = 0f;
        progress.DOFillAmount(targetFill, duration).SetEase(Ease.Linear);
    }
}
