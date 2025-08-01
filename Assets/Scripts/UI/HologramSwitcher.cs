using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HologramSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class AreaMaterial
    {
        public string nameContains;
        public Material hologramMaterialTemplate;
        [HideInInspector] public Material instancedMaterial;
    }

    [System.Serializable]
    public class Floor
    {
        public string name;
        public int floorNumber;
        public GameObject baseModel;
        public GameObject hologramOverlay;
        public List<AreaMaterial> hologramMaterials = new();
    }

    public List<Floor> floors = new List<Floor>();

    [Header("Reveal Settings")]
    public float revealHeight = 10f;
    public float delayBetweenFloors = 0.4f;
    public float durationPerFloor = 1.5f;
    public float blendZone = 1f;
    public Ease animationEase = Ease.InOutSine;

    public bool isHologramActive = false;

    void Start()
    {
        foreach (var floor in floors)
        {
            if (floor.hologramOverlay != null)
            {
                var renderers = floor.hologramOverlay.GetComponentsInChildren<Renderer>();

                foreach (var rend in renderers)
                {
                    AreaMaterial selected = null;

                    // Prioritize longer, more specific nameContains matches
                    foreach (var mat in floor.hologramMaterials)
                    {
                        if (!string.IsNullOrWhiteSpace(mat.nameContains) &&
                            rend.gameObject.name.ToLower().Contains(mat.nameContains.ToLower()))
                        {
                            selected = mat;
                            break;
                        }
                    }

                    // Fallback to first if nothing matched
                    if (selected == null && floor.hologramMaterials.Count > 0)
                    {
                        selected = floor.hologramMaterials[0];
                    }

                    if (selected != null)
                    {
                        if (selected.instancedMaterial == null)
                        {
                            selected.instancedMaterial = new Material(selected.hologramMaterialTemplate);
                            selected.instancedMaterial.SetFloat("_Reveal", 0f);
                            selected.instancedMaterial.SetFloat("_BlendZone", blendZone);
                            print(selected.nameContains);

                        }

                        Material[] mats = new Material[rend.sharedMaterials.Length];
                        for (int i = 0; i < mats.Length; i++)
                            mats[i] = selected.instancedMaterial;

                        rend.materials = mats;
                    }
                }

                floor.hologramOverlay.SetActive(false);
            }

            if (floor.baseModel != null)
                floor.baseModel.SetActive(true);
        }
    }

    [ContextMenu("Reveal Hologram Floor-by-Floor")]
    public void RevealHologram()
    {
        if (isHologramActive) return;

        floors.Sort((a, b) => a.floorNumber.CompareTo(b.floorNumber));

        Sequence seq = DOTween.Sequence();

        foreach (var floor in floors)
        {
            seq.AppendCallback(() =>
            {
                if (floor.baseModel != null)
                {
                    seq.Append(floor.baseModel.transform.DOScaleY(0, durationPerFloor / 2)
                        .SetEase(animationEase)
                        .OnComplete(() =>
                        {
                            floor.baseModel.SetActive(false);
                            if (floor.hologramOverlay != null)
                                floor.hologramOverlay.SetActive(true);
                        }));

                    floor.baseModel.transform.localScale = Vector3.one;
                }
                else
                {
                    if (floor.hologramOverlay != null)
                        floor.hologramOverlay.SetActive(true);
                }

                foreach (var mat in floor.hologramMaterials)
                {
                    if (mat.instancedMaterial != null)
                    {
                        mat.instancedMaterial.SetFloat("_Reveal", 0f);
                        DOTween.To(() => 0f, v => mat.instancedMaterial.SetFloat("_Reveal", v), revealHeight, durationPerFloor)
                            .SetEase(animationEase);
                    }
                }
            });

            seq.AppendInterval(delayBetweenFloors);
        }

        seq.OnComplete(() => isHologramActive = true);
    }

    [ContextMenu("Revert to Base Models Floor-by-Floor")]
    public void RevertToOriginal()
    {
        if (!isHologramActive) return;

        floors.Sort((a, b) => b.floorNumber.CompareTo(a.floorNumber));

        Sequence seq = DOTween.Sequence();

        foreach (var floor in floors)
        {
            seq.AppendCallback(() =>
            {
                foreach (var mat in floor.hologramMaterials)
                {
                    if (mat.instancedMaterial != null)
                    {
                        mat.instancedMaterial.SetFloat("_Reveal", revealHeight);

                        DOTween.To(() => revealHeight, v => mat.instancedMaterial.SetFloat("_Reveal", v), 0f, durationPerFloor)
                            .SetEase(animationEase)
                            .OnComplete(() =>
                            {
                                if (floor.hologramOverlay != null)
                                    floor.hologramOverlay.SetActive(false);

                                if (floor.baseModel != null)
                                {
                                    floor.baseModel.SetActive(true);
                                    floor.baseModel.transform.DOScaleY(1, durationPerFloor / 2)
                                        .SetEase(animationEase);
                                }
                            });
                    }
                }
            });

            seq.AppendInterval(delayBetweenFloors);
        }

        seq.OnComplete(() => isHologramActive = false);
    }

    public void SwitchHologram(bool hologram)
    {
        if (hologram)
            RevealHologram();
        else
            RevertToOriginal();
    }
}
