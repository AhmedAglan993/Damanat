using UnityEngine;

public class GeneralStatisticsManager : MonoBehaviour
{
    public static GeneralStatisticsManager Instance;
    private GeneralStatistics currentStats;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        SimulateGeneralStats();
    }

    void SimulateGeneralStats()
    {
        currentStats = new GeneralStatistics
        {
            averageTraffic = Random.Range(50f, 200f),
            totalVehicles = Random.Range(10, 50),
            totalPeopleInArea = Random.Range(100, 500),
            lastUpdate = System.DateTime.Now.ToString("HH:mm:ss")
        };
    }

    public GeneralStatistics GetStats()
    {
        return currentStats;
    }
}
