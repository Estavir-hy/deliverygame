using Unity.VisualScripting;
using UnityEngine;

public class CaneraFollow : MonoBehaviour
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
        CurrentLooking = Vector3.Lerp(CurrentLooking, TargetCar.position, FollowLerpSmooth);


        transform.position = CurrentLooking + Offset;

        transform.LookAt(CurrentLooking);


    }



     // Vector3 GroundHitPoint()
    // {
    //     Vector3 CamPos = transform.position;
    //     Vector3 RayDir = (TargetCar.position - CamPos).normalized;

    //     float tNumerator = GroundY - CamPos.y;
    //     float tDenominator = RayDir.y;

    //     if(Mathf.Abs(tDenominator) < 1e-4f)
    //     {
    //         return new Vector3(TargetCar.position.x, GroundY, TargetCar.position.z);
    //     }

    //     float t = tNumerator/tDenominator;

    //     if(t < 0)
    //     {
    //         return new Vector3(TargetCar.position.x, GroundY, TargetCar.position.z);
    //     }

    //     Vector3 HitPoint = CamPos + t*RayDir;
    //     return HitPoint;


    // }           super coool math logic to calculate intersection with ground, but so sad it did nothing on this camera
}
