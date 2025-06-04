using UnityEngine;
using UnityEngine.Events;

public class UnityLifecycleEventsComponent : MonoBehaviour
{
    [Header("Lifecycle UnityEvents")]
    [SerializeField] private UnityEvent OnAwake;
    [SerializeField] private UnityEvent OnEnableEvent;
    [SerializeField] private UnityEvent OnStart;
    [SerializeField] private UnityEvent OnDisableEvent;

    private void Awake()
    {
        OnAwake?.Invoke();
    }

    private void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    private void Start()
    {
        OnStart?.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }
}
