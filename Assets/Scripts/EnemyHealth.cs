using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject healthBarPrefab;
    
    private int _currentHealth;

    private Slider _healthBarSlider;
    private Transform _healthBarInstance;

    private void Start()
    {
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