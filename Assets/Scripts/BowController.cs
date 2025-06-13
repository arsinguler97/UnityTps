using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

public class BowController : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform fireArrow;
    [SerializeField] private ArrowConfig arrowConfig;
    [SerializeField] private CinemachineCamera defaultCamera;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private Image crosshairImage;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    
    private float _chargeTime;
    private bool _isCharging;
    

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _isCharging = true;
            _chargeTime = 0f;
            
            cinemachineBrain.DefaultBlend.Time = 3f;
            
            aimCamera.Priority = 20;
            defaultCamera.Priority = 10;

            if (crosshairImage != null)
                crosshairImage.enabled = true;
        }

        if (Input.GetButton("Fire1") && _isCharging)
        {
            _chargeTime += Time.deltaTime;
            _chargeTime = Mathf.Min(_chargeTime, arrowConfig.maxChargeTime);
        }

        if (Input.GetButtonUp("Fire1") && _isCharging)
        {
            FireArrow();
            _isCharging = false;

            cinemachineBrain.DefaultBlend.Time = 1f;
            
            aimCamera.Priority = 10;
            defaultCamera.Priority = 20;
            
            if (crosshairImage != null)
                crosshairImage.enabled = false;
        }
    }
    
    private void FireArrow()
    {
        if (arrowPrefab != null && fireArrow != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, fireArrow.position, fireArrow.rotation);

            float chargeValue = _chargeTime / arrowConfig.maxChargeTime;
            float finalSpeed = Mathf.Lerp(arrowConfig.minSpeed, arrowConfig.maxSpeed, chargeValue);

            arrow.GetComponent<Arrow>().SetInitialSpeed(finalSpeed);
        }
    }
}