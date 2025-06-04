using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LevelBoundingBox : MonoBehaviour
{
    [SerializeField] private string onEnterTagTrigger = "Player";
    [SerializeField] private string onExitTagTrigger = "Interactable";

    [Header("Events")]
    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    [Header("Enter Settings")]
    [SerializeField] private bool triggerEnterOnlyOnce = true;

    private HashSet<GameObject> _enteredObjects = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(onEnterTagTrigger)) return;

        if (triggerEnterOnlyOnce)
        {
            if (_enteredObjects.Contains(other.gameObject)) return;
            _enteredObjects.Add(other.gameObject);
        }

        Debug.Log($"{other.name} entered the boundary!");
        onEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(onExitTagTrigger))
        {
            Debug.Log($"{other.name} exited the boundary!");
            onExit?.Invoke();
        }
    }
}