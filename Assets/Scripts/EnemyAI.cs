using UnityEngine;
using UnityEngine.AI;

public enum EnemyAIState { Idle, Patrol, Chase, Attack, Investigate }

public class BasicEnemyAI : MonoBehaviour
{
    [field: SerializeField] public EnemyAIState CurrentState { get; private set; } = EnemyAIState.Idle;
    [SerializeField] private Transform[] _patrolWaypoints;
    [SerializeField] private float _idleDuration = 5f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _chaseSpeed = 4f;

    private int _currentWaypointIndex = 0;
    private float _idleTimer;
    private NavMeshAgent _agent;
    private Transform _player;
    private Vector3 _lastKnownPlayerPosition;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        ChangeState(EnemyAIState.Idle);
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case EnemyAIState.Idle:
                IdleBehavior();
                break;
            case EnemyAIState.Patrol:
                PatrolBehavior();
                break;
            case EnemyAIState.Chase:
                ChaseBehavior();
                break;
            case EnemyAIState.Attack:
                AttackBehavior();
                break;
            case EnemyAIState.Investigate:
                InvestigateBehavior();
                break;
        }

        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);
            if (distance <= _attackRange)
            {
                ChangeState(EnemyAIState.Attack);
            }
            else
            {
                ChangeState(EnemyAIState.Chase);
            }
        }
    }

    private void IdleBehavior()
    {
        _agent.isStopped = true;
        _idleTimer -= Time.deltaTime;
        if (_idleTimer <= 0f)
        {
            ChangeState(EnemyAIState.Patrol);
        }
    }

    private void PatrolBehavior()
    {
        if (_patrolWaypoints.Length == 0) return;

        _agent.isStopped = false;
        Transform target = _patrolWaypoints[_currentWaypointIndex];
        _agent.SetDestination(target.position);

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _patrolWaypoints.Length;
            ChangeState(EnemyAIState.Idle);
        }
    }

    private void ChaseBehavior()
    {
        if (_player == null)
        {
            ChangeState(EnemyAIState.Patrol);
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(_player.position);
    }

    private void AttackBehavior()
    {
        _agent.isStopped = true;
        transform.LookAt(_player);
    }

    private void InvestigateBehavior()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_lastKnownPlayerPosition);

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            ChangeState(EnemyAIState.Idle);
        }
    }

    public void HandlePlayerDetected(Transform player)
    {
        _player = player;
        ChangeState(EnemyAIState.Chase);
    }

    public void HandlePlayerLost(Vector3 lastSeenPosition)
    {
        _player = null;
        _lastKnownPlayerPosition = lastSeenPosition;
        ChangeState(EnemyAIState.Investigate);
    }

    public void ChangeState(EnemyAIState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;

        if (newState == EnemyAIState.Idle)
        {
            _idleTimer = _idleDuration;
        }
        else if (newState == EnemyAIState.Chase)
        {
            _agent.speed = _chaseSpeed;
        }
        else if (newState == EnemyAIState.Investigate)
        {
            _agent.speed = _chaseSpeed * 0.8f;
        }
    }
}
