using Ricimi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsPopupOpener : PopupOpener
{
    // Start is called before the first frame update
    void Start()
    {
        popup = UIManager.Instance.statisticsPanel.gameObject;
        m_canvas = UIManager.Instance.gameObject.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OpenPopup()
    {
        popup.SetActive(true);
        popup.transform.localScale = Vector3.zero;
        popup.transform.SetParent(m_canvas.transform, false);
        popup.GetComponent<Popup>().Open();
       
    }
}
