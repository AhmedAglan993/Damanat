﻿using UnityEngine;
using DG.Tweening;

public class MapCameraController : MonoBehaviour
{
    public enum CameraMode { Fly, Orbit }
    public CameraMode mode = CameraMode.Orbit;

    [Header("Target Pivot")]
    public Transform pivot;

    [Header("Rotation (Orbit)")]
    public float rotationSpeed = 5f;
    public float rotationDuration = 0.1f;

    [Header("Panning (Orbit)")]
    public float panSpeed = 0.1f;
    public float panDuration = 0.1f;

    [Header("Zoom (Orbit)")]
    public float zoomSpeed = 10f;
    public float minZoom = 10f;
    public float maxZoom = 100f;
    public float zoomDuration = 0.2f;

    [Header("Fly Mode")]
    public float flyMoveSpeed = 10f;
    public float flyLookSensitivity = 2f;

    public bool isFocusActive = false;

    public Vector3 targetAngles;
    private Quaternion targetRotation;
    private float targetDistance;

    private Camera cam;

    void Start()
    {
        if (pivot == null)
        {
            GameObject center = new GameObject("Camera Pivot");
            center.transform.position = Vector3.zero;
            pivot = center.transform;
        }

        cam = Camera.main;
        // 🧠 Set this AFTER the camera is placed
        orbitYaw = transform.eulerAngles.y;
        orbitPitch = transform.eulerAngles.x;
        targetRotation = transform.rotation;
        targetDistance = Vector3.Distance(transform.position, pivot.position);
    }
    void SnapCameraToOrbit()
    {
        transform.position = pivot.position - transform.forward * targetDistance;
    }

    private void ApplyOrbitCameraTransform()
    {
        targetRotation = Quaternion.Euler(orbitPitch, orbitYaw, 0);
        transform.rotation = targetRotation;
        transform.position = pivot.position - targetRotation * Vector3.forward * targetDistance;
    }


    public void OnCameraModeChanged(int index)
    {
        SyncOrbitStateToCurrentCamera();
        mode = (CameraMode)index;
        Debug.Log("Camera mode switched to: " + mode);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mode = (mode == CameraMode.Orbit) ? CameraMode.Fly : CameraMode.Orbit;
            Debug.Log("Camera mode switched to: " + mode);
        }

        if (mode == CameraMode.Orbit)
        {
            HandleRotation();
            HandlePanning();
            HandleZoom();
        }
        else if (mode == CameraMode.Fly)
        {
            HandleFlyMode();
        }
    }
    private float orbitYaw = 0f;   // Horizontal angle (Y axis)
    private float orbitPitch = 0f; // Vertical angle (X axis), clamped
    public void SyncOrbitStateToCurrentCamera()
    {
        targetRotation = transform.rotation;
        targetAngles = transform.eulerAngles;
        targetDistance = Vector3.Distance(transform.position, pivot.position);

        // Also sync orbit yaw/pitch
        orbitYaw = transform.eulerAngles.y;
        orbitPitch = transform.eulerAngles.x;
    }


    void HandleRotation()
    {
        if (isFocusActive)
            return;

        if (Input.GetMouseButton(1)) // Right-click
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotY = -Input.GetAxis("Mouse Y") * rotationSpeed;
            targetDistance = Vector3.Distance(transform.position, pivot.position);

            orbitYaw += rotX;
            orbitPitch += rotY;

            orbitPitch = Mathf.Clamp(orbitPitch, 10f, 80f);

            targetRotation = Quaternion.Euler(orbitPitch, orbitYaw, 0);

            transform.DOKill();
            transform.DORotateQuaternion(targetRotation, rotationDuration).SetUpdate(true);

            Vector3 targetPos = pivot.position - targetRotation * Vector3.forward * targetDistance; // ✅ use fixed distance
            transform.DOMove(targetPos, rotationDuration).SetUpdate(true);
        }
    }



    void HandlePanning()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = -new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")) * panSpeed;
            delta = transform.TransformDirection(delta);

            pivot.DOKill();
            pivot.DOMove(pivot.position + delta, panDuration).SetUpdate(true);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);

            transform.DOKill();
            Vector3 zoomTargetPos = pivot.position - transform.forward * targetDistance;
            transform.DOMove(zoomTargetPos, zoomDuration).SetUpdate(true);
        }
    }

    void HandleFlyMode()
    {
        if (!Input.GetMouseButton(1)) return; // Require right-click to move/look

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * flyLookSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * flyLookSensitivity;
        transform.eulerAngles += new Vector3(mouseY, mouseX, 0);

        // WASD + QE move
        Vector3 input = new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0,
            Input.GetAxis("Vertical")
        );

        transform.Translate(input * flyMoveSpeed * Time.deltaTime, Space.Self);
    }
}
