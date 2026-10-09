using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Collisions
    [SerializeField] private Vector3 boxCheck = new Vector3(1f, 1f, 1f);
    [SerializeField] private LayerMask collisionLayerMask;

    // Colors
    [SerializeField] private Material mat;
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private float currYRot = 0f;
    public bool canPlace { get; private set; }
    
    void Update()
    {
        HandleRotation();
        CanPlace();
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void HandleRotation()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            currYRot += 90f;
            transform.rotation = Quaternion.Euler(transform.rotation.x, currYRot, transform.rotation.y);
        }
    }

    private void CanPlace()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, boxCheck, transform.rotation, collisionLayerMask);

        canPlace = hits.Length == 0;

        mat.color = canPlace ? validColor : invalidColor;
    }



}