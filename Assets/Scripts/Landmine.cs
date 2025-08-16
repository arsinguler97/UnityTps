using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Landmine : MonoBehaviour
{
    [SerializeField] private float explodeDelay = 3f;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private Renderer mineRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private int damageAmount = 3;
    [SerializeField] private AudioClip explosionSFX;

    private Material _originalMaterial;
    private bool _isTriggered;
    private List<Collider> _targetsInTrigger = new List<Collider>();

    private void Start()
    {
        if (mineRenderer != null)
            _originalMaterial = mineRenderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_targetsInTrigger.Contains(other) &&
            (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            _targetsInTrigger.Add(other);
        }

        if (_isTriggered || !other.CompareTag("Player")) return;

        _isTriggered = true;
        StartCoroutine(BlinkAndExplode());
    }

    private void OnTriggerExit(Collider other)
    {
        if (_targetsInTrigger.Contains(other))
            _targetsInTrigger.Remove(other);
    }

    private IEnumerator BlinkAndExplode()
    {
        float timer = 0f;
        float blinkInterval = 0.25f;

        while (timer < explodeDelay)
        {
            if (mineRenderer != null)
                mineRenderer.material = redMaterial;

            yield return new WaitForSeconds(blinkInterval / 2f);

            if (mineRenderer != null)
                mineRenderer.material = _originalMaterial;

            yield return new WaitForSeconds(blinkInterval / 2f);

            timer += blinkInterval;
        }

        foreach (var target in _targetsInTrigger)
        {
            if (target == null) continue;

            if (target.CompareTag("Player"))
            {
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                    playerHealth.TakeDamage(damageAmount);
            }
            else if (target.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damageAmount);
            }
        }

        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        PlayExplosionSound();

        Destroy(gameObject);
    }

    private void PlayExplosionSound()
    {
        if (explosionSFX == null) return;

        GameObject audioObj = new GameObject("ExplosionSound");
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = explosionSFX;
        source.spatialBlend = 1f;
        source.transform.position = transform.position;
        source.Play();
        Destroy(audioObj, explosionSFX.length);
    }
}
