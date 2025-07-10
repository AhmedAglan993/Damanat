using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;
    public Transform buildingTarget;

    [Header("Data")]
    public List<NavigationPoint> emergencyPoints;

    private void Awake()
    {
        Instance = this;
        emergencyPoints = new List<NavigationPoint>();
        emergencyPoints.AddRange(FindObjectsOfType<NavigationPoint>());
       

    }

    public void ShowEmergencyPins()
    {
        foreach (var point in emergencyPoints)
        {

            GameObject pinGO = point.transform.GetChild(0).gameObject;
            PinButton pin = pinGO.GetComponent<PinButton>();
            pinGO.SetActive(true);
            pin.Init(() =>
            {
                Debug.Log($"Navigating from {point.name} to building...");
                StreetPathfinding.Instance.ShowPath(point.transform, buildingTarget);
            });
        }
    }
}
