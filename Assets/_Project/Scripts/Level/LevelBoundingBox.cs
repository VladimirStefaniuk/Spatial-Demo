using UnityEngine;
using UnityEngine.Events;

public class LevelBoundingBox : MonoBehaviour
{
    [SerializeField] private string tagToCheck = "Interactable";
    [SerializeField] private UnityEvent onExit;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagToCheck))
        {
            Debug.Log($"{other.name} exited the boundary!");
            onExit?.Invoke();
        }
    }


}
