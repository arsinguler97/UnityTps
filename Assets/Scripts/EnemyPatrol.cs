using UnityEngine;
using UnityEngine.UI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject healthBarPrefab;

    private Vector3 _startPosition;
    private int _direction = 1;
    private int _currentHealth;

    private Slider _healthBarSlider;
    private Transform _healthBarInstance;

    private void Start()
    {
        _startPosition = transform.position;
        _currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            _healthBarInstance = Instantiate(healthBarPrefab, transform).transform;
            _healthBarInstance.localPosition = new Vector3(0f, 2f, 0f);
            _healthBarSlider = _healthBarInstance.GetComponentInChildren<Slider>();
            _healthBarSlider.maxValue = maxHealth;
            _healthBarSlider.value = maxHealth;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.right * (_direction * speed * Time.deltaTime));

        float distanceFromStart = Vector3.Distance(transform.position, _startPosition);
        if (distanceFromStart >= moveDistance)
        {
            _direction *= -1;
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_healthBarSlider != null)
        {
            _healthBarSlider.value = _currentHealth;
        }

        if (_currentHealth <= 0)
        {
            if (_healthBarInstance != null)
                Destroy(_healthBarInstance.gameObject);

            Destroy(gameObject);
        }
    }
}