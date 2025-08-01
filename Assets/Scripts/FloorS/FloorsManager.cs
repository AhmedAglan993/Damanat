using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorsManager : MonoBehaviour
{
    [SerializeField] public Floor[] floors;
    public static FloorsManager Instance;
    public int CurrentUpFloorNumber;
    public Floor currentUpFloor;
    [HideInInspector] public HologramSwitcher hologramSwitcher;
    [SerializeField] HotSpotButtonsList hotSpotButtonsList;

    public HotSpot currentHotspot;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        hologramSwitcher = GetComponent<HologramSwitcher>();
    }
    void Start()
    {
        //  floors = FindObjectsOfType<Floor>();
        ResetHotSpots();
    }
    public void SelectFloorToShow(int floorNumber)
    {
        foreach (Floor floor in FloorsManager.Instance.floors)
        {
            floor.HotSpotsParent.SetActive(floor.floorNumber == floorNumber);
            FloorsManager.Instance.CurrentUpFloorNumber = floorNumber;

            if (floor.floorNumber > floorNumber)
            {
                floor.RemoveFloor(() =>
                {
                    hotSpotButtonsList.SetHotSpotsList();
                });
            }
            else if (floor.floorNumber == floorNumber)
            {
                print(floorNumber);
               currentUpFloor = floor;

                UIManager.Instance.CurrenthotSpotsHolder = floor.HotSpotsParent.GetComponent<HotSpotsHolder>();
                if (floor.isOutOfBuilding)
                {
                    floor.BackToSBuilding(() =>
                    {
                        floor.GetComponent<FocusAriaInteractable>().OnClick(Vector3.zero);

                    });
                }
                else
                {
                    floor.GetComponent<FocusAriaInteractable>().OnClick(Vector3.zero);
                }
            }
        }
    }
    public void ResetFloorHotspots()
    {
        foreach (var item in UIManager.Instance.CurrenthotSpotsHolder.HotSpots)
        {
            item.gameObject.SetActive(true);
            item.ToggleCeiling(true);
        }
    }
    private void ResetHotSpots()
    {
        CurrentUpFloorNumber = floors.Max(f => f.floorNumber);
        foreach (var floor in floors)
        {
            floor.HotSpotsParent.SetActive(false);
        }
        currentUpFloor = Array.Find(floors, f => f.floorNumber == CurrentUpFloorNumber);
        currentUpFloor.HotSpotsParent.SetActive(true);
        UIManager.Instance.CurrenthotSpotsHolder = currentUpFloor.HotSpotsParent.GetComponent<HotSpotsHolder>();
    }

    public void ResetFloors()
    {
        foreach (Floor floor in floors)
        {
            if (floor.isOutOfBuilding)
            {
                floor.BackToSBuilding();
            }
        }
        ResetHotSpots();
        GetComponent<HologramSwitcher>().RevertToOriginal();
    }

}
