using UnityEngine;
using UnityEngine.AI;

public enum EnemyAIState { Idle, Patrol, Chase, Attack, Investigate, Suspicious, Dead }

public class BasicEnemyAI : MonoBehaviour
{
    [field: SerializeField] public EnemyAIState CurrentState { get; private set; } = EnemyAIState.Idle;
    [SerializeField] private Transform[] _patrolWaypoints;
    [SerializeField] private float _idleDuration = 5f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _chaseSpeed = 4f;
    [SerializeField] private float _suspiciousDuration = 5f;

    private int _currentWaypointIndex = 0;
    private float _idleTimer;
    private float _suspiciousTimer;
    private NavMeshAgent _agent;
    private Transform _player;
    private Vector3 _lastKnownPlayerPosition;
    private Animator _animator;
    private bool _isDead = false;
    private EnemyAIState _previousState;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        ChangeState(EnemyAIState.Idle);
    }

    private void Update()
    {
        if (_isDead) return;

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
            case EnemyAIState.Suspicious:
                SuspiciousBehavior();
                break;
        }

        if (_player != null && CurrentState != EnemyAIState.Suspicious)
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (distance <= _attackRange)
            {
                if (CurrentState != EnemyAIState.Attack)
                    ChangeState(EnemyAIState.Attack);
            }
            else
            {
                if (CurrentState != EnemyAIState.Chase)
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

        if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending)
        {
            _agent.isStopped = true;
        }
    }

    private void AttackBehavior()
    {
        _agent.isStopped = true;
        transform.LookAt(_player);
    }

    private void InvestigateBehavior()
    {
        _agent.isStopped = false;

        if (Vector3.Distance(transform.position, _lastKnownPlayerPosition) > _agent.stoppingDistance)
        {
            _agent.SetDestination(_lastKnownPlayerPosition);
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            ChangeState(EnemyAIState.Idle);
        }
    }

    private void SuspiciousBehavior()
    {
        _agent.isStopped = true;
        _suspiciousTimer -= Time.deltaTime;

        if (_suspiciousTimer <= 0f)
        {
            ChangeState(_previousState);
        }
    }

    public bool IsInAttackState()
    {
        return CurrentState == EnemyAIState.Attack;
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

    public void EnemyTakeDamage()
    {
        if (CurrentState != EnemyAIState.Chase && CurrentState != EnemyAIState.Attack && !_isDead)
        {
            _previousState = CurrentState;
            ChangeState(EnemyAIState.Suspicious);
        }
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;
        _agent.isStopped = true;
        _animator.SetTrigger("Die");
        ChangeState(EnemyAIState.Dead);
    }

    public void ChangeState(EnemyAIState newState)
    {
        if (newState == CurrentState || _isDead) return;

        CurrentState = newState;

        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsRunning", false);
        _animator.ResetTrigger("IsAttacking");

        switch (newState)
        {
            case EnemyAIState.Idle:
                _idleTimer = _idleDuration;
                break;
            case EnemyAIState.Patrol:
                _animator.SetBool("IsWalking", true);
                break;
            case EnemyAIState.Chase:
                _agent.speed = _chaseSpeed;
                _animator.SetBool("IsRunning", true);
                break;
            case EnemyAIState.Investigate:
                _agent.speed = _chaseSpeed * 0.8f;
                _animator.SetBool("IsWalking", true);
                break;
            case EnemyAIState.Attack:
                _animator.SetTrigger("IsAttacking");
                break;
            case EnemyAIState.Suspicious:
                transform.Rotate(0, 180f, 0);
                _suspiciousTimer = _suspiciousDuration;
                break;
        }
    }
}
