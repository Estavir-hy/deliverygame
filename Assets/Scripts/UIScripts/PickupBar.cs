using UnityEngine;
using UnityEngine.UI;

public class PickupBar : MonoBehaviour
{
    [SerializeField] public Image foreSprite;
    [SerializeField] private CargoPickupPoint _cargoPick;
    [SerializeField] public GameObject root;
    private Camera _cam;

    private void LateUpdate()
    {
        if(_cam == null) _cam = Camera.main;
        if(_cam == null) return;
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }

    public void Awake()
    {
        _cargoPick = GetComponentInParent<CargoPickupPoint>();
    }

    public void PickupBarUpdate(float currentTime, float fullTime)
    {
        foreSprite.fillAmount = currentTime/fullTime;
    }

    private void FixedUpdate()
    {
        if(root != null) root.SetActive(_cargoPick.isCounting);
        if(!_cargoPick.isCounting) return;

        PickupBarUpdate(_cargoPick.timer, _cargoPick.StayTime);
    }

}
