using UnityEngine;

public class FocusAriaInteractable : MonoBehaviour
{
    private SmartCameraFocus cameraFocus;

    [Header("Focus Settings")]
    [SerializeField] private Transform viewPivot;
    [SerializeField] private Vector3 rotationPresetEuler = new Vector3(45f, 0, 0f); // Default preset
    [SerializeField] private Vector3 positionPreset = new Vector3(0f, 0f, 0f); // Default preset

    private void Start()
    {
        cameraFocus = Camera.main.GetComponent<SmartCameraFocus>();
    }

    public void OnClick(Transform target)
    {
        cameraFocus.FocusOn(target.localPosition, target.localRotation);
    }

    public void OnClick()
    {
        cameraFocus.FocusOn(viewPivot.position, viewPivot.localRotation);
    }
    GameObject view;
    public void CreateAndAssignPivot()
    {
        if (viewPivot != null) return;

        view = new GameObject($"{gameObject.name}_ViewPivot");
        view.transform.SetParent(transform, false);
        view.transform.localPosition = positionPreset;
        view.transform.localRotation = Quaternion.Euler(rotationPresetEuler);
        viewPivot = view.transform;
    }
    public void AlignPivot()
    {

        viewPivot.transform.localPosition = positionPreset;
        viewPivot.transform.localRotation = Quaternion.Euler(rotationPresetEuler);
    }
    public Transform GetViewPivot() => viewPivot;
    public Quaternion GetRotationPreset() => Quaternion.Euler(rotationPresetEuler);
}
