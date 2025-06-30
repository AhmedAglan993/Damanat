using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] Transform outOfScenePosition;
    [SerializeField] public GameObject HotSpotsParent;
    [SerializeField] private float OutOfSceneduration = 1.5f;
    [SerializeField] public int floorNumber;
    [SerializeField] public string floorName;
    private Vector3 startTransform;
    public bool isOutOfBuilding;
    private void Start()
    {
        startTransform = transform.position;
    }

    public void RemoveFloor(Action onEndRemove)
    {
        transform.DOMove(outOfScenePosition.position, OutOfSceneduration).OnComplete(() =>
        {
            isOutOfBuilding = true;
            onEndRemove.Invoke();
        });
    }

    public void BackToSBuilding(Action onEndBack)
    {
        transform.DOMove(startTransform, OutOfSceneduration).OnComplete(() =>
        {
            isOutOfBuilding = false;
            onEndBack.Invoke();
        });

    }
    public void BackToSBuilding()
    {
        transform.DOMove(startTransform, OutOfSceneduration).OnComplete(() =>
        {
            isOutOfBuilding = false;
        });

    }
}
