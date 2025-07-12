using Ricimi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorSelector : MonoBehaviour
{
    [SerializeField] GameObject floorSelectorButtonPrefabe;
    [SerializeField] Transform floorSelectorsListParent;
    [SerializeField] HotSpotButtonsList hotSpotButtonsList;
    List<GameObject> cleanButtons;
    [SerializeField] TextMeshProUGUI floorSelectorButtonOpenerTxt;
    // Start is called before the first frame update
    void Start()
    {
        PopulateFlooersData();
    }
    void PopulateFlooersData()
    {
        cleanButtons = new List<GameObject>();

        for (int i = 0; i < FloorsManager.Instance.floors.Length; i++)
        {
            cleanButtons.Add(Instantiate(floorSelectorButtonPrefabe, floorSelectorsListParent));
        }
        for (int i = 0; i < cleanButtons.Count; i++)
        {
            int index = i;
            cleanButtons[i].GetComponent<CleanButton>().onClick.AddListener(() =>
            {
                SelectFloorToShow(FloorsManager.Instance.floors[index].floorNumber);
                floorSelectorButtonOpenerTxt.text = FloorsManager.Instance.floors[index].floorName;
            });
            cleanButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = FloorsManager.Instance.floors[i].floorName;
        }
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
               FloorsManager.Instance.currentUpFloor = floor;

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
        GetComponent<Popup>().Close();
    }
}
