using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HotspotMasterPlan;

public class HotSpotsHolder : MonoBehaviour
{
    [SerializeField] public HotSpot[] HotSpots;
    private void Start()
    {
        HotSpots = GetComponentsInChildren<HotSpot>();
    }
    [ContextMenu("SetHotSpots")]
    public void SetHotSpots()
    {
        HotSpots = GetComponentsInChildren<HotSpot>();
    }
    [ContextMenu("SetNames")]
    public void SetNames()
    {
        for (int i = 0; i < HotSpots.Length; i++)
        {
            HotSpots[i].gameObject.name = HotSpots[i].hotSpotName;

        }
    }
}
