using UnityEngine;

public class EmergencyMapHighlighter : MonoBehaviour
{
    public static EmergencyMapHighlighter Instance;

    public GameObject buildingHologramVisual;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowHologram(bool show)
    {
        if (buildingHologramVisual)
            buildingHologramVisual.SetActive(show);
    }
}
