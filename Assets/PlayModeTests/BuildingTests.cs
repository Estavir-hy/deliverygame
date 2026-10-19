using System.Collections;
using NUnit.Framework;
using Unity.Netcode;  
using UnityEngine;
using UnityEngine.TestTools;

public class BuildingTests : NetworkTestBase
{
    [UnityTest]
    public IEnumerator PlaceBuilding_FarmsSpendsAndRejectsInsufficientMaterials()
    {
        yield return StartHost();

        var baseGo = SpawnNetworked("Base");
        var mat = baseGo.AddComponent<MaterialSystem>();
        baseGo.GetComponent<NetworkObject>().Spawn();


        yield return new WaitForSeconds(1.2f);
        Assert.GreaterOrEqual(mat.MaterialNum.Value, 1);


        mat.enabled = false;

        var playerGo = SpawnNetworked("Player");
        var bm = playerGo.AddComponent<BuildingManager>();
        bm.solidPrefab = SpawnNetworked("WallTemplate");
        playerGo.GetComponent<NetworkObject>().Spawn();

        // have no enough material
        mat.MaterialNum.Value = 5;
        bm.RequestPlaceServerRpc(new Vector3(5, 0, 5), Quaternion.identity);

        Assert.AreEqual(5, mat.MaterialNum.Value);
        Assert.IsNull(GameObject.Find("WallTemplate(Clone)"));

        // succeed create wall
        mat.MaterialNum.Value = 25;
        bm.RequestPlaceServerRpc(new Vector3(5, 0, 5), Quaternion.identity);

        Assert.AreEqual(15, mat.MaterialNum.Value);
        Assert.IsNotNull(GameObject.Find("WallTemplate(Clone)"));
    }
}