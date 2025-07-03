using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private BasicEnemyAI _enemyAI;
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _fieldOfViewAngle = 120f;
    [SerializeField] private LayerMask _detectionLayer;

    private Transform _player;
    private bool _playerInSight = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        if (_player == null) return;

        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        bool canSeePlayer = false;

        if (angleToPlayer <= _fieldOfViewAngle * 0.5f)
        {
            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
            if (Physics.Raycast(ray, out RaycastHit hit, _detectionRange, _detectionLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    canSeePlayer = true;
                    Debug.DrawRay(transform.position + Vector3.up, directionToPlayer * _detectionRange, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position + Vector3.up, directionToPlayer * _detectionRange, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(transform.position + Vector3.up, directionToPlayer * _detectionRange, Color.red);
            }
        }

        if (canSeePlayer && !_playerInSight)
        {
            _playerInSight = true;
            _enemyAI.HandlePlayerDetected(_player);
        }
        else if (!canSeePlayer && _playerInSight)
        {
            _playerInSight = false;
            Vector3 lastSeen = transform.position + directionToPlayer * _detectionRange;
            _enemyAI.HandlePlayerLost(lastSeen);
        }
    }
}
