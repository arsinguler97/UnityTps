using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private ArrowConfig arrowConfig;

    private Vector3 _currentVelocity;
    private bool _hasHit;
    private float _initialSpeed;

    private void Start()
    {
        float speedToUse = _initialSpeed > 0 ? _initialSpeed : arrowConfig.speed;
        _currentVelocity = transform.forward * speedToUse;

        ArrowManager.Instance?.RegisterArrow(gameObject);
    }

    private void Update()
    {
        if (_hasHit) return;

        _currentVelocity += Vector3.down * (arrowConfig.gravity * Time.deltaTime);
        transform.position += _currentVelocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(_currentVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        _hasHit = true;
        transform.position += transform.forward * 0.2f;

        if (other.CompareTag(arrowConfig.enemyTag))
        {
            transform.SetParent(other.transform);
        }

        this.enabled = false;
    }
    
    public void SetInitialSpeed(float speed)
    {
        _initialSpeed = speed;
    }
}