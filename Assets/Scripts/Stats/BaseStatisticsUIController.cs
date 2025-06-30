using UnityEngine;

public abstract class BaseStatisticsUIController : MonoBehaviour
{
    public void ShowStatistics<T>(T stats) where T : class
    {
        if (stats == null) return;

        switch (stats)
        {
            case RoomStatistics room:
                ShowRoomStatistics(room);
                break;

            case GeneralStatistics general:
                ShowGeneralStatistics(general);
                break;

            default:
                Debug.LogWarning($"Unknown statistics type: {typeof(T)}");
                break;
        }
    }

    protected abstract void ShowRoomStatistics(RoomStatistics stats);
    protected abstract void ShowGeneralStatistics(GeneralStatistics stats);
}
