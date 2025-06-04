using UnityEngine;

[RequireComponent(typeof(Animation))]
public class BarrierDoor : MonoBehaviour
{
    [SerializeField] private Animation animaton;
    [SerializeField] private string doorOpenAnimation = "BarrierOpen";
    
    private bool isOpen = false;
    
    private void Reset()
    {
        if(!animaton) { animaton = GetComponent<Animation>(); }
    }

    public void Open()
    {
        if (isOpen) 
            return;
        
        var doorCollider = GetComponent<Collider>();
        if(doorCollider) doorCollider.enabled = false;
        animaton.Play(doorOpenAnimation);
        isOpen = true;
    }
}
