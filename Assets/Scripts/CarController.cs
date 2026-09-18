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
    private float ThrustInput; // "w/space"positive   "s"negative
    private float RotateInput; // "A/D"

    [Header("ground detect")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            CameraFollow cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.TargetCar = this.transform;
            }
        }
    }

    void GatherInputs()
    {
        bool IsAccelerate = Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.Space);
        bool IsBrake = Input.GetKey(KeyCode.S);

        ThrustInput = 0;
        if(IsAccelerate) ThrustInput = 1f;
        if(IsBrake) ThrustInput = -1f;        //brake will overlap

        RotateInput = Input.GetAxisRaw("Horizontal");   // A/D input

        float forwardSpeed = Vector3.Dot(RB.linearVelocity,transform.forward);

        if(forwardSpeed < -0.1f)
        {
            RotateInput = -RotateInput;
        }
    }

    void Rotation()
    {
        float torque = RotateInput * RotateTorque;
        RB.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
    }

    void ThrustAndBrake()
    {
        Vector3 forceDir = transform.forward;
        forceDir.y = 0;
        if(ThrustInput>0)
        {
            RB.AddForce(forceDir * ThrustForce, ForceMode.Acceleration);
        }
        if(ThrustInput<0)
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


    void Update()
    {
        if(!IsOwner) return;
        GatherInputs();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        Rotation();
        ThrustAndBrake();
        ClampMaxSpeed();
        CustomFriction();
    }
}
