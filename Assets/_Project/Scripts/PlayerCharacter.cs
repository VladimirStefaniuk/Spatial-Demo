using KinematicCharacterController;
using UnityEngine;

public struct CharacterInput
{
    public Quaternion Rotation;
    
    public Vector2 Move;
    
    public bool Jump;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform cameraTarget;

    [Space, Header("Config")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float gravity = -90f;
    
    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump; 
    
    public void Initialize()
    {
        motor.CharacterController = this;
    }
 

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation; 
        
        // create 3d XZ movement vector from 2d input
        _requestedMovement = new Vector3(input.Move.x, 0, input.Move.y);
    
        // clamp movement to 1, so moving diagonally doesn't overshoot, and doesn't make character to move faster that usual
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1);
        // orient vector to character's look rotation
       _requestedMovement = input.Rotation * _requestedMovement;

       _requestedJump = input.Jump || _requestedJump;
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane(
            _requestedRotation * Vector3.forward,    
            motor.CharacterUp) * _requestedMovement.magnitude;
        
        if(forward != Vector3.zero)
        {
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    { 
        if(motor.GroundingStatus.IsStableOnGround)
        {
            var groundedMovement =
                motor.GetDirectionTangentToSurface(_requestedMovement, motor.GroundingStatus.GroundNormal);
            
            currentVelocity = groundedMovement * walkSpeed;
            
            if (_requestedJump)
            {
                _requestedJump = false;
            
                // Unstick the player from the ground
                motor.ForceUnground(time: 0f);
            
                // set minimum vertical speed to the jump speed
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
      
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
        }
        // else in the air
        else
        {
            // Gravity
            currentVelocity += motor.CharacterUp * gravity * deltaTime;
        }

       
    }

    public void BeforeCharacterUpdate(float deltaTime)
    { 
    }

    public void PostGroundingUpdate(float deltaTime)
    { 
    }

    public void AfterCharacterUpdate(float deltaTime)
    { 
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    { 
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    { 
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
        Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    { 
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    { 
    }
    
    public Transform GetCameraTarget() => cameraTarget;
}
