using Ricimi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotSpotButtonsList : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] CleanButton hotSpotListButtonPrefab;
    [SerializeField] Transform hotSpotsButtonsParent;
    [SerializeField] List<CleanButton> hotSpotsButtons = new List<CleanButton>();

    private void OnEnable()
    {
       SetHotSpotsList();
    }
    public void SetHotSpotsList()
    {
        ResetList();
        for (int i = 0; i < UIManager.Instance.CurrenthotSpotsHolder.HotSpots.Length; i++)
        {
            hotSpotsButtons.Add(Instantiate(hotSpotListButtonPrefab, hotSpotsButtonsParent));
            int index = i;
            hotSpotsButtons[i].GetComponent<PopupOpener>().m_canvas = UIManager.Instance.GetComponent<Canvas>();
            hotSpotsButtons[i].GetComponent<PopupOpener>().popup = UIManager.Instance.statisticsPanel.gameObject;
            AssignOnClick(index);
            hotSpotsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = UIManager.Instance.CurrenthotSpotsHolder.HotSpots[i].hotSpotName;
        }
    }

    private void AssignOnClick( int index)
    {
        hotSpotsButtons[index].onClick.AddListener(() =>
        {
            UIManager.Instance.CurrenthotSpotsHolder.HotSpots[index].GetComponent<FocusAriaInteractable>().OnClick();
            hotSpotsButtons[index].GetComponent<PopupOpener>().OpenPopup();
            GetComponent<Popup>().Close();

            RoomStatistics roomStatistics = new RoomStatistics(UIManager.Instance.CurrenthotSpotsHolder.HotSpots[index].hotSpotName);

            hotSpotsButtons[index].GetComponent<PopupOpener>().popup.GetComponent<RoomStatisticsUIController>().ShowStatistics(roomStatistics);
        });
    }

    private void ResetList()
    {
        for (int i = 0; i < hotSpotsButtons.Count; i++)
        {
            Destroy(hotSpotsButtons[i].gameObject);
        }
        hotSpotsButtons.Clear();
        hotSpotsButtons = new List<CleanButton>();
    }
}
