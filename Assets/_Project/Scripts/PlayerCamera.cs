using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private bool invertY = true;
    [SerializeField] private float sensitivity = 0.3f;
    private Vector3 _cachedEulerAngles;

    public void Initialize(Transform cameraTarget)
    {
        Cursor.lockState = CursorLockMode.Locked;

        _cachedEulerAngles = cameraTarget.rotation.eulerAngles;
        
        transform.position = cameraTarget.position;
        transform.rotation = cameraTarget.rotation;
    }

    public void UpdateRotation(CameraInput input)
    {
        _cachedEulerAngles += new Vector3(invertY ? -input.Look.y : input.Look.y, input.Look.x) * sensitivity;
        transform.eulerAngles = _cachedEulerAngles;
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }

    public Camera GetCamera() => playerCamera;
    
    public Vector3 GetRotationEulerAngles() => _cachedEulerAngles;
}
