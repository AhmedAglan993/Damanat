using Ricimi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorSelector : MonoBehaviour
{
    public static FloorSelector Instance;
    [SerializeField] GameObject floorSelectorButtonPrefabe;
    [SerializeField] Transform floorSelectorsListParent;
    List<GameObject> cleanButtons;
    [SerializeField] TextMeshProUGUI floorSelectorButtonOpenerTxt;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
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
                FloorsManager.Instance.SelectFloorToShow(FloorsManager.Instance.floors[index].floorNumber);
                floorSelectorButtonOpenerTxt.text = FloorsManager.Instance.floors[index].floorName;
                GetComponent<Popup>().Close();

            });
            cleanButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = FloorsManager.Instance.floors[i].floorName;
        }
    }

}
