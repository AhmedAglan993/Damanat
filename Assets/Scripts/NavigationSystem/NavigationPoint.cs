using UnityEngine;

public enum EmergencyType
{
    FireStation,
    Ambulance
}

public class NavigationPoint : MonoBehaviour
{
    public string name;
    public EmergencyType type;
    public GameObject visualPin; // Reference to the spawned pin object
}
