using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    [SerializeField] public KeyCode build = KeyCode.B;
    [SerializeField] public KeyCode interact = KeyCode.E;

    [SerializeField] public KeyCode thrust = KeyCode.W;
    [SerializeField] public KeyCode brake = KeyCode.S;

    [SerializeField] private KeyCode rotateBuilding = KeyCode.R;
    [SerializeField] private KeyCode placeBuilding = KeyCode.Mouse0;



    public event System.Action<float, float> OnMoveInput;

    public event System.Action OnInteractPressed;

    public event System.Action OnBuildTogglePressed;

    public event System.Action OnRotateBuildingPressed;

    public event System.Action OnPlaceBuildingPressed;

    private void Update()
    {
        if (!IsOwner) return;

        ReadMoveInput();
        ReadActionInput();
    }

    private void ReadMoveInput()
    {
        bool IsAccelerate = Input.GetKey(thrust);
        bool IsBrake = Input.GetKey(brake);

        float ThrustInput = 0;
        if(IsAccelerate) ThrustInput = 1f;
        if(IsBrake) ThrustInput = -1f;        //brake will overlap

        float RotateInput = Input.GetAxisRaw("Horizontal");

        OnMoveInput?.Invoke(ThrustInput, RotateInput);
    }

    private void ReadActionInput()
    {
        if (Input.GetKeyDown(interact))
            OnInteractPressed?.Invoke();

        if (Input.GetKeyDown(build))
            OnBuildTogglePressed?.Invoke();

        if (Input.GetKeyDown(rotateBuilding))
            OnRotateBuildingPressed?.Invoke();

        if (Input.GetKeyDown(placeBuilding))
            OnPlaceBuildingPressed?.Invoke();
    }
}
