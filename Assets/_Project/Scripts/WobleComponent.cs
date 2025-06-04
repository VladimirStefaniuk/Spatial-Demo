using UnityEngine;

public class Wobble : MonoBehaviour
{
    [Header("Rotation Wobble")]
    public float wobbleSpeed = 5f;
    public float wobbleAmount = 10f;
    public Vector3 wobbleAxis = new Vector3(0, 0, 1);

    [Header("Position Jitter")]
    public float positionJitterAmount = 0.05f;
    public float positionJitterSpeed = 2f;

    private Quaternion _originalRotation;
    private Vector3 _originalPosition;

    void Start()
    {
        _originalRotation = transform.localRotation;
        _originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Rotational wobble
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        transform.localRotation = _originalRotation * Quaternion.AngleAxis(wobble, wobbleAxis.normalized);

        // Positional jitter
        float x = Mathf.PerlinNoise(Time.time * positionJitterSpeed, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, Time.time * positionJitterSpeed) - 0.5f;
        float z = Mathf.PerlinNoise(Time.time * positionJitterSpeed, Time.time * 0.5f) - 0.5f;

        Vector3 jitter = new Vector3(x, y, z) * positionJitterAmount;
        transform.localPosition = _originalPosition + jitter;
    }
}