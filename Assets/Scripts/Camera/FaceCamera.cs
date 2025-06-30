using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    public float smoothSpeed = 5f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Quaternion targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
