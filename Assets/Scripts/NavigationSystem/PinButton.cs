using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PinButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI label;
    public Image icon;

    private Action onClickCallback;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    // ✅ Call this when creating the button from NavigationManager
    public void Init(Action callback)
    {
        onClickCallback = callback;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickCallback?.Invoke());
        }
    }
}
