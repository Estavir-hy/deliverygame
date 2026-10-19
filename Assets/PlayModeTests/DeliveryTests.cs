using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DeliveryTests : NetworkTestBase
{
    [UnityTest]
    public IEnumerator Deliver_AtOtherBaseRejected_AtOwnBaseAccepted()
    {
        yield return StartHost();

        var truck = SpawnTruck();
        var cargo = truck.GetComponent<PlayerCargoState>();
        var score = truck.GetComponent<PlayerScore>();

        // get the cargo
        cargo.PickupCargoServerRpc();
        Assert.AreEqual(PlayerTruckState.loaded, cargo.CurrentState.Value);


        var otherBase = CreateDeliveryBase("OtherBase", cargo.OwnerClientId + 1, new Vector3(10, 0, 0));
        var ownBase    = CreateDeliveryBase("OwnBase",   cargo.OwnerClientId,     new Vector3(-10, 0, 0));

        // ① must be reject for other base
        truck.transform.position = new Vector3(10, 0, 0);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        otherBase.Interact();

        Assert.AreEqual(PlayerTruckState.loaded, cargo.CurrentState.Value);
        Assert.AreEqual(0, score.Score.Value);

        // succeed if it s own base
        truck.transform.position = new Vector3(-10, 0, 0);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        ownBase.Interact();

        Assert.AreEqual(PlayerTruckState.Empty, cargo.CurrentState.Value, "自己基地交付失败");
        Assert.AreEqual(5, score.Score.Value);
    }


    private CargoDeliveryPoint CreateDeliveryBase(string name, ulong ownerId, Vector3 pos)
    {
        var go = new GameObject(name);
        Track(go);
        go.transform.position = pos;
        go.AddComponent<BoxCollider>().isTrigger = true;

        var ownership = go.AddComponent<BaseOwnership>();
        ownership.OwnerPlayerId.Value = ownerId;

        return go.AddComponent<CargoDeliveryPoint>();
    }
}