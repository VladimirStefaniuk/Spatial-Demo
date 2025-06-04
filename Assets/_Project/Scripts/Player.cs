using System;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private CrosshairController crosshairController;
 
     
    [SerializeField] private float _interactibilityDistance = 100;
    [SerializeField] private float swipeSampleWindowSeconds = 0.3f;  
    [SerializeField] private  float throwSpeed = 1f;
    
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    
    private Camera _camera;
     
    private Vector3 _screenCenterVector;
    private RaycastHit _currentlyHitObject;
    private Vector3 _offset;
    private Plane _dragPlane; 
    private Transform _selectedObject;
    private Vector2 _cachedMoveInput;
    private Vector2 _cachedLookInput;
    
    private Vector3 _dragTargetOffsetFromRayOrigin;

    private float _dragDistance;
    private readonly List<(Vector2 position, float time)> _lookHistory = new();

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    { 
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    private void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
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
       
        // Track look input history for throw velocity
        _lookHistory.Add((_cachedLookInput, Time.time));

        // Remove old entries
        while (_lookHistory.Count > 0 && Time.time - _lookHistory[0].time > swipeSampleWindowSeconds)
        {
            _lookHistory.RemoveAt(0);
        }
 
        UpdateLook();
    }

    private void FixedUpdate()
    {
        DragSelectedObject();
    }
 
    private void LateUpdate()
    {
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
    }
     
    private void DragSelectedObject()
    {
        if (_selectedObject == null) return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Vector3 targetPosition = ray.origin + ray.direction * _dragDistance + _dragTargetOffsetFromRayOrigin;

        Rigidbody rb = _selectedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 toTarget = targetPosition - rb.position;
            float velocityMultiplier = 10f;

            rb.linearVelocity = toTarget * velocityMultiplier;
            rb.angularVelocity = Vector3.zero;
        }
    }
 
    private void UpdateLook()
    { 
        // is dragging
        if (_attackAction.IsPressed() && _selectedObject)
        {
            return;
        } 
        
        if (_attackAction.WasReleasedThisFrame())
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
                    Vector2 swipeDelta = Vector2.zero;
                    if (_lookHistory.Count >= 2)
                    {
                        swipeDelta = _cachedLookInput - _lookHistory[0].position;
                    }

                    // Project to world space using camera orientation
                    float timeDelta = Time.time - _lookHistory[0].time;
                    Vector2 swipeVelocity = swipeDelta / timeDelta;
                    float throwForce = swipeVelocity.magnitude * throwSpeed;
                    Vector3 worldSwipeDirection = (_camera.transform.right * swipeVelocity.x +
                                                   _camera.transform.up * swipeVelocity.y).normalized;

                    rb.linearVelocity = worldSwipeDirection * throwForce;

                    // Add torgue
                    Vector3 torqueDirection =
                        Vector3.Cross(_camera.transform.forward, worldSwipeDirection).normalized;
                    float torqueStrength = throwForce * 0.1f;

                    rb.AddTorque(torqueDirection * torqueStrength, ForceMode.Impulse);
                } 
            }
            
            _selectedObject = null;
        }
        else
        { 
            var canInteract = false;

            // show current state of cursor
            var crosshairRay = _camera.ViewportPointToRay(_screenCenterVector);
            if (Physics.Raycast(crosshairRay, out _currentlyHitObject, _interactibilityDistance))
            {
                if (_currentlyHitObject.collider.CompareTag(Data.GameConstants.InteractableTag))
                { 
                    canInteract = true;
                }
            }

            if (_attackAction.WasPressedThisFrame() && canInteract)
            {
                crosshairController.SetState(CrosshairController.State.Active);

                _selectedObject = _currentlyHitObject.transform;

                // Set up Rigidbody
                Rigidbody rb = _selectedObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.constraints = RigidbodyConstraints.FreezeRotation;

                // Calculate drag distance and initial offset from camera ray
                Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hitInfo, _interactibilityDistance))
                {
                    _dragDistance = Vector3.Distance(_camera.transform.position, hitInfo.point);
                    _dragTargetOffsetFromRayOrigin = _selectedObject.position - (ray.origin + ray.direction * _dragDistance);
                }
            }
            else if (canInteract)
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
