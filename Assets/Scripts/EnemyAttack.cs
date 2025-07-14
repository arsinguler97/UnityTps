using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float _attackDistance;
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void DealDamage()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance <= _attackDistance)
        {
            PlayerHealth playerHealth = _player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damageAmount);
        }
    }
}


