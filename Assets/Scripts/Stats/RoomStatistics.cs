using UnityEngine;

[System.Serializable]
public class RoomStatistics
{
    public string roomName;
    public float occupancy;       // %
    public float energyUsage;     // kWh
    public float temperature;     // °C
    public float airQuality;      // AQI
    public int visitors;
    public string lastMaintenance;

    public RoomStatistics(string name)
    {
        roomName = name;
        SimulateData();
    }

    public void SimulateData()
    {
        occupancy = Random.Range(20f, 95f);
        energyUsage = Random.Range(10f, 100f);
        temperature = Random.Range(18f, 28f);
        airQuality = Random.Range(30f, 100f);
        visitors = Random.Range(0, 200);
        lastMaintenance = System.DateTime.Now.AddDays(-Random.Range(1, 30)).ToString("yyyy-MM-dd");
    }
}
