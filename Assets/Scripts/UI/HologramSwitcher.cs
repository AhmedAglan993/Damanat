using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HologramSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Floor
    {
        public string name;
        public int floorNumber;
        public GameObject baseModel;              // Real geometry
        public GameObject hologramOverlay;        // Overlay with mesh only
        public Material hologramMaterialTemplate; // Template used to create instance

        [HideInInspector] public Material instancedMaterial;
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
            // Create unique material instance from template
            if (floor.hologramMaterialTemplate != null)
            {
                floor.instancedMaterial = new Material(floor.hologramMaterialTemplate);
                floor.instancedMaterial.SetFloat("_Reveal", 0f);
                floor.instancedMaterial.SetFloat("_BlendZone", blendZone);
            }

            // Assign instanced material to all renderers in overlay
            if (floor.hologramOverlay != null)
            {
                var renderers = floor.hologramOverlay.GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers)
                {
                    Material[] mats = new Material[rend.sharedMaterials.Length];
                    for (int i = 0; i < mats.Length; i++)
                        mats[i] = floor.instancedMaterial;

                    rend.materials = mats;
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
                // Scale down base model then hide it and show hologram overlay
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

                    // Ensure scale is reset for next time
                    floor.baseModel.transform.localScale = Vector3.one;
                }
                else
                {
                    if (floor.hologramOverlay != null)
                        floor.hologramOverlay.SetActive(true);
                }

                if (floor.instancedMaterial != null)
                {
                    floor.instancedMaterial.SetFloat("_Reveal", 0f);

                    DOTween.To(() => 0f, v => floor.instancedMaterial.SetFloat("_Reveal", v), revealHeight, durationPerFloor)
                        .SetEase(animationEase);
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
        floors.Sort((a, b) => b.floorNumber.CompareTo(a.floorNumber)); // reverse order

        Sequence seq = DOTween.Sequence();

        foreach (var floor in floors)
        {
            seq.AppendCallback(() =>
            {
                if (floor.instancedMaterial != null)
                {
                    floor.instancedMaterial.SetFloat("_Reveal", revealHeight);

                    DOTween.To(() => revealHeight, v => floor.instancedMaterial.SetFloat("_Reveal", v), 0f, durationPerFloor)
                        .SetEase(animationEase)
                        .OnComplete(() =>
                        {
                            if (floor.hologramOverlay != null)
                                floor.hologramOverlay.SetActive(false);

                            if (floor.baseModel != null)
                            {
                                floor.baseModel.SetActive(true);
                              //  floor.baseModel.transform.localScale = Vector3.zero;

                                // Scale up base model
                                floor.baseModel.transform.DOScaleY(1, durationPerFloor / 2)
                                    .SetEase(animationEase);
                            }
                        });
                }
                else
                {
                    if (floor.baseModel != null)
                        floor.baseModel.SetActive(true);
                }
            });

            seq.AppendInterval(delayBetweenFloors);
        }

        seq.OnComplete(() => isHologramActive = false);
    }
    public void SwitchHologram(bool hologram)
    {
        if (hologram)
        {
            RevealHologram();

        }
        else
        {
            RevertToOriginal();
        }
    }
}
