using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider hologramSlider;
    private void Start()
    {
        hologramSlider.value = FloorsManager.Instance.hologramSwitcher.isHologramActive ? 1 : 0;
    }
   
}
