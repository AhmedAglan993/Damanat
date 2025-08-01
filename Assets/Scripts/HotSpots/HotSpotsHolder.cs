using UnityEngine;

public class HotSpotsHolder : MonoBehaviour
{
    [SerializeField] public HotSpot[] HotSpots;
    public int floorNumber => transform.parent.GetComponent<Floor>().floorNumber;
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
