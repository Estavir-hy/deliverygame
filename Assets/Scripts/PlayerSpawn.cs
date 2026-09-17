using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject playerCarPrefab;
    public Transform SpawnPoint;

    private GameObject currentPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if(currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(playerCarPrefab, SpawnPoint.position,SpawnPoint.rotation);

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam!= null)
        {
            cam.TargetCar = currentPlayer.transform;
        }
    }

    
}
