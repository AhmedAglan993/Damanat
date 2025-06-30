using System.Collections.Generic;
using UnityEngine;

public class RoomStatisticsManager : MonoBehaviour
{
    public static RoomStatisticsManager Instance;

    private Dictionary<string, RoomStatistics> statisticsByRoom = new();

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Simulate for common room names (must match your hotspot names!)
        string[] roomNames = { "MeetingRoomA", "Kitchen", "OpenSpace1", "Garage", "CafeRoof", "Basement1", "Basement2" };

        foreach (string room in roomNames)
        {
            statisticsByRoom[room] = new RoomStatistics(room);
        }
    }

    public RoomStatistics GetStats(string roomName)
    {
        if (statisticsByRoom.TryGetValue(roomName, out var stats))
            return stats;

        Debug.LogWarning($"No stats found for room: {roomName}");
        return null;
    }
}
