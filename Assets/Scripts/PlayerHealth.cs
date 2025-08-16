using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AudioClip deathSound;

    private int _currentHealth;
    private Animator _animator;
    private bool _isDead = false;
    private PlayerMovement _movement;

    private void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();

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

            PlayDeathSound();

            _animator.SetTrigger("Die");

            if (_movement != null)
                _movement.enabled = false;

            StartCoroutine(WaitForDeathAnim());
        }
    }

    private void PlayDeathSound()
    {
        if (deathSound == null) return;

        GameObject audioObj = new GameObject("PlayerDeathSound");
        AudioSource src = audioObj.AddComponent<AudioSource>();
        src.clip = deathSound;
        src.spatialBlend = 1f;
        src.transform.position = transform.position;
        src.Play();
        Destroy(audioObj, deathSound.length);
    }

    private IEnumerator WaitForDeathAnim()
    {
        yield return new WaitForSeconds(3f);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameOverMenu");
    }
}
