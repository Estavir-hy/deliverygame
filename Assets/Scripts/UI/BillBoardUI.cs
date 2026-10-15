using UnityEngine;

public class BillBoardUI : MonoBehaviour
{
   private Camera _cam;

    private void LateUpdate()
    {
        if(_cam == null) _cam = Camera.main;
        if(_cam == null) return;
        transform.forward = _cam.transform.forward;
    }
}
