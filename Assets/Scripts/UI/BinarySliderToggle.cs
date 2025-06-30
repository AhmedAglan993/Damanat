using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

[RequireComponent(typeof(Slider))]
public class BinarySliderToggle : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.wholeNumbers = false;      // Ensure only 0 or 1
        slider.minValue = 0;
        slider.maxValue = 1;
    }

    public void OnPointerClick()
    {
        // Toggle between 0 and 1
        float newValue = slider.value == 0 ? 1 : 0;
        slider.SetValueWithoutNotify(newValue);
        SwitchToHologram(newValue);
    }

    public void SwitchToHologram(float hologramOn)
    {
        bool hologram = hologramOn == 1 ? true : false;
        FloorsManager.Instance.hologramSwitcher.SwitchHologram(hologram);
    }
}
