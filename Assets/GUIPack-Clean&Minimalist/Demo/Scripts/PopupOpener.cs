// Copyright (C) 2016 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace Ricimi
{
    // This class is responsible for creating and opening a popup of the given prefab and add
    // it to the UI canvas of the current scene.
    public class PopupOpener : MonoBehaviour
    {
        public GameObject popup;

      [SerializeField]  public Canvas m_canvas;

        public virtual void OpenPopup()
        {
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(m_canvas.transform, false);
            popup.GetComponent<Popup>().Open();
        }
        public virtual void OpenPopup(bool toggle)
        {
            toggle = popup.activeSelf;

            if (toggle)
            {
                popup.GetComponent<Popup>().Close();
            }
            else
            {
                popup.SetActive(true);
                popup.GetComponent<Popup>().Open();

            }
        }
    }
}
