using System;
using UnityEngine; 
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private CrosshairController crosshairController;


    public float throwSpeed = 5f;
    
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    
    private Camera _camera;
     
    private Vector3 _screenCenterVector;
    private RaycastHit _currentlyHitObject;
    private Vector3 _offset;
    private Plane _dragPlane;
    
    private Vector2 _initialLookPosition; 


    [SerializeField] private float _interactibilityDistance = 100;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    { 
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    private void Start()
    {
        _camera = playerCamera.GetCamera();
        _screenCenterVector = new Vector3(0.5f, 0.5f, 0f);
        
        // Input system setup
        var actionMap = inputActions.FindActionMap("Player");
        
        _moveAction = actionMap.FindAction("Move");
        _lookAction = actionMap.FindAction("Look");
        _jumpAction = actionMap.FindAction("Jump");
        _attackAction = actionMap.FindAction("Attack"); 
    }
 

    // Update is called once per frame
    void Update()
    {
        _cachedLookInput = _lookAction.ReadValue<Vector2>();
        _cachedMoveInput = _moveAction.ReadValue<Vector2>();
        
        // get input and update camera
        var cameraInput = new CameraInput() { Look = _cachedLookInput};
        playerCamera.UpdateRotation(cameraInput);
  
        // get input to update character 
        playerCharacter.UpdateInput(new CharacterInput()
        {
            Rotation = playerCamera.transform.rotation,
            Move = _cachedMoveInput,
            Jump = _jumpAction.WasPressedThisFrame(),
        });
        
        Vector2 currentLook = _lookAction.ReadValue<Vector2>();
        _lookDelta = currentLook - _lastLookPosition;
        _lastLookPosition = currentLook;


        UpdateLook();
    }

    private void FixedUpdate()
    {
        DragSelectedObject();
    }
    
    private void DragSelectedObject()
    {
        if (_selectedObject == null) return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPosition = ray.GetPoint(enter) + _offset;

            Rigidbody rb = _selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 toTarget = targetPosition - rb.position;
                float velocityMultiplier = 10f; // Increase for faster response

                rb.linearVelocity = toTarget * velocityMultiplier;

                // Optional: dampen angular velocity
                rb.angularVelocity = Vector3.zero;
            }
        }
    }


 
    private void LateUpdate()
    {
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
    }

    private Transform _selectedObject;
    private Vector2 _cachedMoveInput;
    private Vector2 _cachedLookInput;

    private Vector2 _initialLook;
    
    private Vector3 _initialHoldOffset;
    private float _zOffsetModifier = 0f;
    [SerializeField] private float _pushPullSpeed = 2f;
    private Vector3 _initialPlayerPosition;

    
    private Vector2 _lastLookPosition;
    private Vector2 _lookDelta;
    private float _mouseDeltaSmoothing = 0.1f;
    public float dragSpeed = 1f;
    
    private void UpdateLook()
    {
        if (_attackAction.IsPressed() && _selectedObject)
        { 
            // _lookDelta = (_cachedLookInput - _lastLookPosition) / Time.deltaTime;
            // _lastLookPosition = _cachedLookInput;
            //
            // crosshairController.SetState(CrosshairController.State.Active);
            //
            // // Dynamically update drag plane using current camera forward, at updated distance
            // Vector3 dragPlaneOrigin = transform.position + transform.forward * (_initialHoldOffset.magnitude + _zOffsetModifier);
            // _dragPlane = new Plane(-_camera.transform.forward, dragPlaneOrigin);
            //
            // // Update z offset based on player movement
            // _zOffsetModifier += _cachedMoveInput.y * _pushPullSpeed * Time.deltaTime;
            //
            // // Compute the target world position from the cursor
            // Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // if (_dragPlane.Raycast(ray, out float enter))
            // {
            //     Vector3 targetPosition = ray.GetPoint(enter) + _offset;
            //
            //     Rigidbody rb = _selectedObject.GetComponent<Rigidbody>();
            //     if (rb != null)
            //     {
            //         // Use MovePosition for physics-aware movement
            //         rb.MovePosition(targetPosition);
            //     }
            // }
        } 
        else if (_attackAction.WasReleasedThisFrame())
        {
            crosshairController.SetState(CrosshairController.State.Idle);
            
            if(_selectedObject)
            {
                var rb = _selectedObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = false;
                    rb.useGravity = false; // simulate zero gravity
                    rb.linearVelocity = Vector3.zero;
                    rb.constraints = RigidbodyConstraints.None;

                    // Convert look delta to screen-space swipe direction
                    Vector2 swipeDelta = _cachedLookInput - _initialLook;

                    // Project to world space using camera orientation
                    Vector3 worldSwipeDirection = (_camera.transform.right * swipeDelta.x +
                                                   _camera.transform.up * swipeDelta.y).normalized;

                    float swipeMagnitude = swipeDelta.magnitude;
                    float throwForce = swipeMagnitude * throwSpeed;
 
                    rb.linearVelocity = worldSwipeDirection * throwForce;
                    
                    // Add torgue
                    Vector3 torqueDirection = Vector3.Cross(_camera.transform.forward, worldSwipeDirection).normalized;
                    float torqueStrength = throwForce * 0.1f;

                    rb.AddTorque(torqueDirection * torqueStrength, ForceMode.Impulse); 
                }

            }
            
            _selectedObject = null;
        }
        else
        { 
            var _canInteract = false;

            // show current state of cursor
            var crosshairRay = _camera.ViewportPointToRay(_screenCenterVector);
            if (Physics.Raycast(crosshairRay, out _currentlyHitObject, _interactibilityDistance))
            {
                if (_currentlyHitObject.collider.CompareTag(Data.GameConstants.InteractableTag))
                { 
                    _canInteract = true;
                }
            }

            if (_attackAction.WasPressedThisFrame() && _canInteract)
            {
                crosshairController.SetState(CrosshairController.State.Active);
                
                
                _selectedObject = _currentlyHitObject.transform;
                
                // make selected object floatable
                Rigidbody rb = _selectedObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                // rb.linearDamping = 0f;
                // rb.angularDamping = 0f;
                //     
                // Lock drag to XY plane (Z stays constant)
                // Use a drag plane perpendicular to camera
                _dragPlane = new Plane(-_camera.transform.forward, _selectedObject.position);


                
                // Create a plane at the object's position for dragging
                // _dragPlane = new Plane(Vector3.forward, _selectedObject.transform.position);

                // Calculate the offset between object and mouse hit point
                _dragPlane.Raycast(crosshairRay, out float enter);
                    
                _offset = _selectedObject.transform.position - crosshairRay.GetPoint(enter);
                
                // Cache offset from player to object when picking up
                _initialHoldOffset = _selectedObject.transform.position - transform.position;
                _initialPlayerPosition = transform.position;
                _zOffsetModifier = 0f;

                _initialLook = _cachedLookInput;
                _initialLookPosition = _lookAction.ReadValue<Vector2>();
                _lastLookPosition = _initialLookPosition;

            }
            else if (_canInteract)
            {
                crosshairController.SetState(CrosshairController.State.Interactable);
            }
            else
            {
                crosshairController.SetState(CrosshairController.State.Idle);
            }
        } 
    }
}
