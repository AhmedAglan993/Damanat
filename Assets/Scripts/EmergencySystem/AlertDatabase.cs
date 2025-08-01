using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class AlertDatabase
{
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "alerts.json");

    private static List<AlertEntry> cachedAlerts;

    static AlertDatabase()
    {
        cachedAlerts = LoadAlerts();
    }

    public static void SaveAlert(AlertEntry newAlert)
    {
        cachedAlerts.Add(newAlert);
        SaveAll(cachedAlerts);
    }

    public static List<AlertEntry> LoadAlerts()
    {
        if (!File.Exists(filePath))
            return new List<AlertEntry>();

        string json = File.ReadAllText(filePath);
        AlertListWrapper wrapper = JsonUtility.FromJson<AlertListWrapper>(json);
        return wrapper?.entries ?? new List<AlertEntry>();
    }

    private static void SaveAll(List<AlertEntry> alerts)
    {
        AlertListWrapper wrapper = new AlertListWrapper { entries = alerts };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    [System.Serializable]
    private class AlertListWrapper
    {
        public List<AlertEntry> entries;
    }
}
