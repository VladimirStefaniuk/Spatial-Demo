using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
 
    [Space, Header("Config")]
    [SerializeField] private float _interactibilityDistance = 100;
    [SerializeField] private Camera _camera;

    
    private bool _canInteract; 
    
    private Vector3 _screenCenterVector;
    private RaycastHit _currentlyHitObject;
    
   
    //
    // private void Update()
    // {
    //    
    //     if (Input.GetButton("Fire1") && _interactingObject)
    //     {
    //         crosshairController.SetState(CrosshairController.State.Active);
    //     }
    //     else if (Input.GetButtonUp("Fire1"))
    //     {
    //         crosshairController.SetState(CrosshairController.State.Idle);
    //     }
    //     else
    //     {
    //         bool clicked = Input.GetButtonDown("Fire1");
    //
    //     
    //     } 
    // }
    
    
}