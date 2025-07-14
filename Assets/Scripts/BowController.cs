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
    [SerializeField] private float maxOverchargeDuration = 3f;
    [SerializeField] private float aimSpreadAmount = 2f;
    [SerializeField] private Animator _animator;

    private float _chargeTime;
    private bool _isCharging;
    private float _overchargeTime;
    private bool _canCharge = true;
    private Vector3 _originalCrosshairPos;
    private PlayerMovement _player;

    public bool IsCharging => _isCharging;

    private void Start()
    {
        if (crosshairImage != null)
            _originalCrosshairPos = crosshairImage.rectTransform.anchoredPosition;

        _player = FindFirstObjectByType<PlayerMovement>();

        aimCamera.Priority = 10;
        defaultCamera.Priority = 20;

        if (crosshairImage != null)
            crosshairImage.enabled = false;
    }

    private void Update()
    {
        if (_player == null) return;

        if (crosshairImage != null)
            crosshairImage.enabled = _player.IsBowEquipped;

        if (!_player.IsBowEquipped) return;

        if (Input.GetButtonDown("Fire1") && _canCharge && _player.currentArrowCount > 0)
        {
            _isCharging = true;
            _chargeTime = 0f;
            _overchargeTime = 0f;

            _animator.SetBool("IsDrawing", true);
            _animator.ResetTrigger("TriggerFire");

            cinemachineBrain.DefaultBlend.Time = 3f;
            aimCamera.Priority = 20;
            defaultCamera.Priority = 10;
        }

        if (Input.GetButton("Fire1") && _isCharging)
        {
            _chargeTime += Time.deltaTime;

            if (_chargeTime >= arrowConfig.maxChargeTime)
            {
                _overchargeTime += Time.deltaTime;
                ShakeCrosshair();

                if (_overchargeTime >= maxOverchargeDuration)
                {
                    AutoFireArrow();
                }
            }
        }

        if (Input.GetButtonUp("Fire1") && _isCharging)
        {
            _isCharging = false;

            _animator.SetBool("IsDrawing", false);
            _animator.SetTrigger("TriggerFire");

            FireArrow(false);
        }
    }

    private void AutoFireArrow()
    {
        _animator.SetBool("IsDrawing", false);
        _animator.SetTrigger("TriggerFire");
        FireArrow(true);
    }

    private void FireArrow(bool autoFired)
    {
        _isCharging = false;
        _chargeTime = Mathf.Min(_chargeTime, arrowConfig.maxChargeTime);
        _overchargeTime = 0f;

        cinemachineBrain.DefaultBlend.Time = 0.75f;
        aimCamera.Priority = 10;
        defaultCamera.Priority = 20;

        if (crosshairImage != null)
        {
            crosshairImage.enabled = false;
            crosshairImage.rectTransform.anchoredPosition = _originalCrosshairPos;
        }

        _canCharge = false;
        Invoke(nameof(EnableCharging), cinemachineBrain.DefaultBlend.Time);

        if (arrowPrefab != null && fireArrow != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, fireArrow.position, fireArrow.rotation);
            ArrowManager.Instance?.RegisterArrow(arrow);

            float chargeValue = _chargeTime / arrowConfig.maxChargeTime;
            float finalSpeed = Mathf.Lerp(arrowConfig.minSpeed, arrowConfig.maxSpeed, chargeValue);

            Vector3 spread = Vector3.zero;

            if (autoFired)
            {
                float intensity = Mathf.Clamp(_overchargeTime, 0f, maxOverchargeDuration);
                float dynamicSpread = aimSpreadAmount * (1f + intensity);

                spread = new Vector3(
                    Random.Range(-dynamicSpread, dynamicSpread),
                    Random.Range(-dynamicSpread, dynamicSpread),
                    0f
                );
            }

            arrow.transform.rotation *= Quaternion.Euler(spread);

            Arrow arrowScript = arrow.GetComponent<Arrow>();
            if (arrowScript != null)
            {
                arrowScript.SetInitialSpeed(finalSpeed);
            }

            _player.currentArrowCount--;
        }
    }

    private void EnableCharging()
    {
        _canCharge = true;
    }

    private void ShakeCrosshair()
    {
        if (crosshairImage == null) return;

        float intensity = Mathf.Clamp(_overchargeTime, 0f, maxOverchargeDuration);
        float shakeAmount = Mathf.Sin(Time.time * 40f) * (1f + intensity * 3f);
        float verticalShake = Mathf.Cos(Time.time * 35f) * (1f + intensity * 2f);
        Vector3 shake = new Vector3(shakeAmount, verticalShake, 0f);
        crosshairImage.rectTransform.anchoredPosition = _originalCrosshairPos + shake;
    }
}
