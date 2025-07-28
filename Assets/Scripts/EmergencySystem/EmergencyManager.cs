using System.Collections.Generic;
using UnityEngine;

public class EmergencyManager : MonoBehaviour
{
    public GameObject pathMarkerPrefab;
    private List<GameObject> markers = new();

    public void ShowExitPathFrom(Transform user)
    {
        ClearPath();

        var path = EmergencyPathfinder.Instance.GetPath(user);
        foreach (var node in path)
        {
            GameObject marker = Instantiate(pathMarkerPrefab, node.transform.position + Vector3.up * 0.2f, Quaternion.identity);
            markers.Add(marker);
        }

        EmergencyMapHighlighter.Instance.ShowHologram(true); // Trigger hologram
    }

    public void ClearPath()
    {
        foreach (var obj in markers)
            Destroy(obj);
        markers.Clear();

        EmergencyMapHighlighter.Instance.ShowHologram(false); // Hide if needed
    }
}
