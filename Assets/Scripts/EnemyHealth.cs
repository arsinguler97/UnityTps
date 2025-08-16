using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject healthBarPrefab;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip zombieGroan;
    [SerializeField] private AudioClip zombieDeath;

    private int _currentHealth;
    private bool _isDead;

    private Slider _healthBarSlider;
    private Transform _healthBarInstance;
    private Animator _animator;
    private BasicEnemyAI _enemyAI;

    private void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        _enemyAI = GetComponent<BasicEnemyAI>();

        if (_audioSource != null && zombieGroan != null)
        {
            _audioSource.clip = zombieGroan;
            _audioSource.loop = true;
            _audioSource.Play();
        }

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

            if (_audioSource != null)
            {
                _audioSource.Stop();
                _audioSource.loop = false;
                _audioSource.PlayOneShot(zombieDeath);
            }

            if (_healthBarInstance != null)
                Destroy(_healthBarInstance.gameObject);

            _animator.SetTrigger("Die");

            if (_enemyAI != null)
                _enemyAI.Die();

            UnparentAllArrows();
            Destroy(gameObject, 3f);
        }
    }

    private void UnparentAllArrows()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Arrow"))
            {
                child.SetParent(null);
            }
        }
    }

    public bool IsDead()
    {
        return _isDead;
    }
}
