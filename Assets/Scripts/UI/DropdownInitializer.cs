using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownInitializer : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int defaultIndex = 2;

    void Start()
    {
       
        if (dropdown != null && dropdown.options.Count > defaultIndex)
        {
            dropdown.value = defaultIndex;
            dropdown.RefreshShownValue();
        }
    }
}
