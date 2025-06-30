using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HotspotDataSO", menuName = "Hotspots/Hotspot Data", order = 1)]
public class HotspotMasterPlan : ScriptableObject
{
    [System.Serializable]
    public class Hotspot
    {
        public string name;
        public Vector2 normalizedPosition; // X and Y from 0–1 (relative to image)
    }

    public List<Hotspot> hotspots = new List<Hotspot>();
}
