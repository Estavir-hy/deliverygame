using UnityEngine;

public class BuildingManager : MonoBehaviour
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

        if(MaterialSystem.Instance.MaterialNum.Value <= 10)
        {
            Debug.Log("Not enough materials to build!");
            return;
        }
        
       MaterialSystem.Instance.BuyWall(10);

        Instantiate(solidPrefab, fm.transform.position, fm.transform.rotation);

        CancelBuild();
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
}
