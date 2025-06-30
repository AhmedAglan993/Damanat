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
