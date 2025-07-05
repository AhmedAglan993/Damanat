using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using System;

public class SmartCameraFocus : MonoBehaviour
{
    [Header("View Settings")]
    public Vector3 viewOffset = new Vector3(0, 20f, -20f);
    public float moveDuration = 1.5f;
    public Ease ease = Ease.InOutSine;

    [Header("Defaults")]
    public Vector3 defaultTargetPosition = Vector3.zero;
    public Vector3 defaultViewOffset = new Vector3(0, 20f, -20f);
    public Vector3 defaultRotationEuler = new Vector3(45f, 45f, 0f); // Used only in Reset

    [Header("References")]
    public MapCameraController mapCameraController; // Assign in Inspector

    private Tween moveTween;
    private Tween lookTween;
    [SerializeField, Range(0f, 10f)] float waitToStartCamera;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(waitToStartCamera);
     //   ResetCamera();
    }

    /// <summary>
    /// Focuses on a target position and uses a given world rotation.
    /// </summary>
    public void FocusOn(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 desiredPosition = targetPosition + viewOffset;

        moveTween?.Kill();
        lookTween?.Kill();

        if (mapCameraController != null)
            mapCameraController.isFocusActive = true;

        moveTween = transform.DOMove(desiredPosition, moveDuration).SetEase(ease);

        lookTween = DOVirtual.DelayedCall(0.1f, () =>
        {
            var rotTween = transform.DORotate(targetRotation.eulerAngles, moveDuration).SetEase(ease);
            rotTween.OnComplete(() =>
            {
                SyncMapController(targetPosition);
                mapCameraController.SyncOrbitStateToCurrentCamera();

            });
        });
    }

    public void ResetCamera()
    {
        Vector3 resetTarget = defaultTargetPosition;
        Vector3 desiredPosition = resetTarget + defaultViewOffset;
        Quaternion resetRotation = Quaternion.Euler(defaultRotationEuler);

        moveTween?.Kill();
        lookTween?.Kill();

        if (mapCameraController != null)
            mapCameraController.isFocusActive = true;

        moveTween = transform.DOMove(desiredPosition, moveDuration).SetEase(ease);

        lookTween = DOVirtual.DelayedCall(0.1f, () =>
        {
            var rotTween = transform.DORotate(resetRotation.eulerAngles, moveDuration).SetEase(ease);
            rotTween.OnComplete(() =>
            {
                SyncMapController(resetTarget);
                mapCameraController.SyncOrbitStateToCurrentCamera();    
            });
        });
    }

    private void SyncMapController(Vector3 targetPosition)
    {
        if (mapCameraController != null)
        {
            mapCameraController.targetAngles = transform.eulerAngles;
            mapCameraController.pivot.position = targetPosition;
            mapCameraController.isFocusActive = false;
        }
    }
}
