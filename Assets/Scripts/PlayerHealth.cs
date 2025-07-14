using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject gameOverUI;

    private int _currentHealth;
    private Animator _animator;
    private bool _isDead = false;

    private void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;

        if (_animator != null)
            _animator.SetTrigger("GetHit");

        if (healthSlider != null)
            healthSlider.value = _currentHealth;

        if (_currentHealth <= 0)
        {
            _isDead = true;
            _animator.SetTrigger("Die");

            if (gameOverUI != null)
                gameOverUI.SetActive(true);
        }
    }
}