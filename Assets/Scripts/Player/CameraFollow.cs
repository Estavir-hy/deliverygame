using Unity.Microsoft.GDK;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow : MonoBehaviour
{
    [Header("FollowTarget")]
    public Transform TargetCar;
    [Header("offset")]
    public Vector3 Offset = new Vector3(0,7,-9);
    [Header("FollowLerpSmooth")]
    public float FollowLerpSmooth = 0.5f;

    public float GroundY = 0;
    
    public Vector3 CurrentLooking;

    void FixedUpdate()
    {
        // Find the local player's car
        if (TargetCar == null)
        {
            if (NetworkManager.Singleton == null)
                return;

            if (!NetworkManager.Singleton.IsClient)
                return;

            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

            if (playerObject == null)
                return;

            TargetCar = playerObject.transform;
            CurrentLooking = TargetCar.position;
        }

        // Follow the local player's car
        CurrentLooking = Vector3.Lerp( CurrentLooking, TargetCar.position, FollowLerpSmooth );

        transform.position = CurrentLooking + Offset;
        transform.LookAt(CurrentLooking);
    }
}
