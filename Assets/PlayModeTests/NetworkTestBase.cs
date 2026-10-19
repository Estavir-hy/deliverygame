using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.TestTools;

public class NetworkTestBase
{
    protected NetworkManager Nm;
    private readonly List<GameObject> _spawned = new();

    protected IEnumerator StartHost()
    {
        var go = new GameObject("NetworkManager");
        Track(go);
        var transport = go.AddComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777);

        Nm = go.AddComponent<NetworkManager>();
        Nm.NetworkConfig = new NetworkConfig { NetworkTransport = transport };

        Assert.IsTrue(Nm.StartHost());
        yield return null; 
    }

    protected void Track(GameObject go) => _spawned.Add(go);


    protected GameObject SpawnNetworked(string name)
    {
        var go = new GameObject(name);
        Track(go);

        var netObj = go.AddComponent<NetworkObject>();
        var hashField = typeof(NetworkObject).GetField("GlobalObjectIdHash",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(hashField);
        hashField.SetValue(netObj, 0x6B726C78u);
        return go;
    }
    protected GameObject SpawnTruck()
    {
        var truck = SpawnNetworked("Truck");
        truck.AddComponent<Rigidbody>().useGravity = false;
        truck.AddComponent<BoxCollider>();

        var cargo = truck.AddComponent<PlayerCargoState>();
        cargo.CargoIndecator = truck.AddComponent<MeshRenderer>();
        truck.AddComponent<PlayerScore>();

        truck.GetComponent<NetworkObject>().Spawn();
        return truck;
    }


    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if (Nm != null)
        {
            Nm.Shutdown();
            yield return new WaitForSeconds(0.1f);
        }

        foreach (var go in _spawned)
            if (go != null) Object.Destroy(go);
        
        
        _spawned.Clear();
        yield return null;
    }
}
