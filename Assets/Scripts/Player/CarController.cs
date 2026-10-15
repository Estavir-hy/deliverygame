using System.Text.RegularExpressions;
using Unity.Netcode;
using UnityEngine;


public class CarController : NetworkBehaviour
{
    [Header("MovementData")]
    [Tooltip("Thrustforce")]
    public float ThrustForce = 8f;
    [Tooltip("Brakeforce")]
    public float BrakeForce = 12f;
    [Tooltip("Rotatetorque")]
    public float RotateTorque = 2f;
    [Tooltip("Maxspeed")]
    public float MaxSpeed = 7f;
    [Tooltip("Friction")]
    public float Friction = 1.5f;

    public Rigidbody RB;

    private InputManager _playerInput;

    [Header("ground detect")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    

    private float _cachedThrust;
    private float _cachedRotate;
    private MatchManager matchManager;

    public override void OnNetworkSpawn()
    {
        matchManager = FindAnyObjectByType<MatchManager>();

        if (IsOwner)
        {
            CameraFollow cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.TargetCar = this.transform;
            }

            _playerInput = GetComponent<InputManager>();
            _playerInput.OnMoveInput += HandleMoveInput;
        }
    }

    
    public override void OnNetworkDespawn()
    {
        if (_playerInput != null)
        {
            _playerInput.OnMoveInput -= HandleMoveInput;
        }
        base.OnNetworkDespawn();
    }

    public void HandleMoveInput(float thrust, float rotate)
    {
        if (matchManager != null && matchManager.MatchEnded.Value) return;

        if(LobbyManager.Instance != null && !LobbyManager.Instance.isStarted.Value) return;
        

        _cachedThrust = thrust;
        _cachedRotate = rotate;
    }

    void Rotation(float rotate)
    {
        float forwardSpeed = Vector3.Dot(RB.linearVelocity,transform.forward);

        if(forwardSpeed < -0.1f)
        {
            rotate = -rotate;
        }
        float torque = rotate * RotateTorque;
        RB.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
    }

    void ThrustAndBrake(float thrust)
    {
        Vector3 forceDir = transform.forward;
        forceDir.y = 0;
        if(thrust>0)
        {
            RB.AddForce(forceDir * ThrustForce, ForceMode.Acceleration);
        }
        if(thrust<0)
        {
            RB.AddForce(-forceDir * BrakeForce, ForceMode.Acceleration);
        }
    }

    void ClampMaxSpeed()
    {
        Vector3 flatVelocity = RB.linearVelocity;
        flatVelocity.y = 0;

        float currentSpeed = flatVelocity.magnitude;

        if(currentSpeed > MaxSpeed)
        {
            flatVelocity = flatVelocity.normalized * MaxSpeed;
            RB.linearVelocity = new Vector3(flatVelocity.x, RB.linearVelocity.y, flatVelocity.z);
        }
    }

    void CustomFriction()
    {
        bool isGrounded = Physics.Raycast(
        transform.position, 
        Vector3.down, 
        groundCheckDistance, 
        groundLayer
        );
        if(isGrounded)
        {
            Vector3 flatVel = RB.linearVelocity;
            flatVel.y = 0;
            RB.AddForce(-flatVel * Friction, ForceMode.Acceleration);   
        }
    }

    public float GetCurrentSpeed()
    {
        Vector3 V = RB.linearVelocity;
        V.y = 0;
        return V.magnitude;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        if (matchManager != null && matchManager.MatchEnded.Value) return;

        Rotation(_cachedRotate);
        ThrustAndBrake(_cachedThrust);
        ClampMaxSpeed();
        CustomFriction();
    }
}
