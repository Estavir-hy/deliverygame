using UnityEngine;
using Unity.Netcode;
using System.Xml.Serialization;

public class BuildingManager : NetworkBehaviour
{
    public static BuildingManager instance;

    // Camera
    [SerializeField] private Camera mainCam;
    // Prefabs
    [SerializeField] private GameObject hollowPrefab;
    [SerializeField] private GameObject solidPrefab;

    private FollowMouse fm;
    private Plane ground = new Plane(Vector3.up, Vector3.zero);
    public bool IsBuilding => fm != null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        if (mainCam == null)
        {
            mainCam = Camera.main;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsBuilding)
            {
                CancelBuild();
            }
            else
            {
                StartBuilding();
            }
        }

        if(IsBuilding)
        {
            UpdatePreview();
            if(Input.GetMouseButtonDown(0))
            {
                TryPlaceBuilding();
            }
        }
        
    }

    private void StartBuilding()
    {
        Vector3 spawnPoint = GetWorldMousePosition();
        GameObject previewObj = Instantiate(hollowPrefab, spawnPoint, Quaternion.identity);
        fm = previewObj.GetComponent<FollowMouse>();
    }

    private void CancelBuild()
    {
        if (fm != null)
        {
            Destroy(fm.gameObject);
            fm = null;
        }
    }

    private void UpdatePreview()
    {
        Vector3 mousePos = GetWorldMousePosition();
        fm.SetPosition(new Vector3(mousePos.x, 2.1f, mousePos.z));
    }

    private void TryPlaceBuilding()
    {
        if(!fm.canPlace)
        {
            Debug.Log("Cannot place building here!");
            return;
        }
        
        RequestPlaceServerRpc(fm.transform.position, fm.transform.rotation);

        CancelBuild();
    }
    [ServerRpc]
    private void RequestPlaceServerRpc(Vector3 position, Quaternion rotation)
    {
        MaterialSystem pb = GetLocalPlayerBase(OwnerClientId);

        if(pb == null)
        {
            Debug.Log($"No base found for player {OwnerClientId}!");
            return;
        }

        if(pb.MaterialNum.Value < 10)
        {
            Debug.Log($"Not enough materials to place building! Current: {pb.MaterialNum.Value}");
            return;
        }

        pb.BuyWall(10);

        GameObject wall = Instantiate(solidPrefab, position, rotation);
        wall.GetComponent<NetworkObject>().Spawn();
    }
    private Vector3 GetWorldMousePosition()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (ground.Raycast(ray, out float dist))
        {
            return ray.GetPoint(dist);
        }
        return Vector3.zero;
    }

    private MaterialSystem GetLocalPlayerBase(ulong clientId)
    {
        MaterialSystem[] allBases = FindObjectsByType<MaterialSystem>();
        
        if(allBases.Length == 1)
        {
            return allBases[0];
        }

        foreach(MaterialSystem pb in allBases)
        {
            if (pb.BaseOwnerId.Value == clientId)
            {
                return pb;
            }
        }

        BaseOwnership[] allOwnerships = FindObjectsByType<BaseOwnership>();
        foreach(BaseOwnership bo in allOwnerships)
        {
            if (bo.OwnerPlayerId.Value == clientId)
            {
                MaterialSystem found = bo.GetComponent<MaterialSystem>() 
                    ?? bo.GetComponentInParent<MaterialSystem>() 
                    ?? bo.GetComponentInChildren<MaterialSystem>();
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}
