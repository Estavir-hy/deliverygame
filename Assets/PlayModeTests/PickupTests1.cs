using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PickupTests1 : NetworkTestBase
{
    [UnityTest]
    public IEnumerator PickupAfterWait_LoadsCargo()
    {
        yield return StartHost();

        // scene pickup point trigger
        var point = new GameObject("PickupPoint");
        Track(point);
        point.AddComponent<BoxCollider>().isTrigger = true;
        var pickup = point.AddComponent<CargoPickupPoint>();
        pickup.StayTime = 0.1f;

        // drive to the pickup area
        var truck = SpawnTruck();
        var cargo = truck.GetComponent<PlayerCargoState>();


        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // E to interact

        pickup.Interact();

        // waiting time
        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(PlayerTruckState.loaded, cargo.CurrentState.Value);
        Assert.AreEqual(5, cargo.CargoHealth.Value);
    }
}
