using System.Collections.Generic;
using UnityEngine;

public class NavigationPathRenderer : MonoBehaviour
{
    public enum PathStyle { Line, Arrow, Glow }
    public PathStyle currentStyle;
    public Material lineMaterial, glowMaterial, arrowMaterial;

    private LineRenderer currentRenderer;

    public void RenderPath(List<StreetNode> path)
    {
        if (currentRenderer != null) Destroy(currentRenderer.gameObject);

        GameObject line = new GameObject("NavPath");
        currentRenderer = line.AddComponent<LineRenderer>();
        currentRenderer.material = GetMaterialForStyle();
        currentRenderer.widthMultiplier = 0.4f;
        currentRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
            currentRenderer.SetPosition(i, path[i].transform.position + Vector3.up * 0.2f);

        // Optional: add dot/arrow movement for effects
    }

    Material GetMaterialForStyle()
    {
        return currentStyle switch
        {
            PathStyle.Line => lineMaterial,
            PathStyle.Arrow => arrowMaterial,
            PathStyle.Glow => glowMaterial,
            _ => lineMaterial
        };
    }

    public void SetStyle(int index)
    {
        currentStyle = (PathStyle)index;
    }
}
