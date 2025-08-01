using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class AlertDatabase
{
    private static readonly string filePath = Application.persistentDataPath + "/alerts.json";

    public static void SaveAlerts(List<AlertEntry> alerts)
    {
        string json = JsonUtility.ToJson(new AlertListWrapper { entries = alerts });
        File.WriteAllText(filePath, json);
    }

    public static List<AlertEntry> LoadAlerts()
    {
        if (!File.Exists(filePath))
            return new List<AlertEntry>();

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<AlertListWrapper>(json).entries;
    }

    [System.Serializable]
    private class AlertListWrapper
    {
        public List<AlertEntry> entries;
    }
}
