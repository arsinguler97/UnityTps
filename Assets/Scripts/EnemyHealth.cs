using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject healthBarPrefab;

    private int _currentHealth;
    private bool _isDead = false;

    private Slider _healthBarSlider;
    private Transform _healthBarInstance;
    private Animator _animator;
    private BasicEnemyAI _enemyAI;

    private void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        _enemyAI = GetComponent<BasicEnemyAI>();

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
        if (_isDead) return;

        _currentHealth -= amount;

        if (_healthBarSlider != null)
            _healthBarSlider.value = _currentHealth;

        if (_enemyAI != null && !_isDead)
            _enemyAI.EnemyTakeDamage();

        if (_currentHealth <= 0)
        {
            _isDead = true;

            if (_healthBarInstance != null)
                Destroy(_healthBarInstance.gameObject);

            _animator.SetTrigger("Die");

            if (_enemyAI != null)
                _enemyAI.Die();

            Destroy(gameObject, 3f);
        }
    }

    public bool IsDead()
    {
        return _isDead;
    }
}