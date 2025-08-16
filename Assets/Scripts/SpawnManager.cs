using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject zombiePrefab;

    [SerializeField] private int maxCoins = 10;
    [SerializeField] private int maxMines = 10;
    [SerializeField] private int maxZombies = 10;

    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private float coinYOffset = 0.3f;
    [SerializeField] private float mineYOffset = -0.3f;

    private Transform[] _patrolPoints;
    private Transform _player;
    private float _timer;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        _patrolPoints = new Transform[4];
        _patrolPoints[0] = GameObject.Find("PatrolPoint1")?.transform;
        _patrolPoints[1] = GameObject.Find("PatrolPoint2")?.transform;
        _patrolPoints[2] = GameObject.Find("PatrolPoint3")?.transform;
        _patrolPoints[3] = GameObject.Find("PatrolPoint4")?.transform;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < checkInterval) return;
        _timer = 0f;

        if (CountRootObjectsWithTag("Coin") < maxCoins)
        {
            SpawnAtRandomNavMeshLocation(coinPrefab, coinYOffset);
        }

        if (CountRootObjectsWithTag("Mine") < maxMines)
        {
            SpawnAtRandomNavMeshLocation(minePrefab, mineYOffset);
        }

        if (CountRootObjectsWithTag("Enemy") < maxZombies)
        {
            SpawnZombieAtRandomPatrolPoint();
        }
    }

    private int CountRootObjectsWithTag(string tag)
    {
        int count = 0;
        GameObject[] all = GameObject.FindGameObjectsWithTag(tag);
        foreach (var obj in all)
        {
            if (obj.transform.parent == null)
                count++;
        }
        return count;
    }

    private void SpawnAtRandomNavMeshLocation(GameObject prefab, float yOffset)
    {
        Vector3 spawnPos;
        int attempts = 10;

        do
        {
            Vector3 samplePos = RandomNavMeshLocation(20f);
            if (NavMesh.SamplePosition(samplePos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                spawnPos = hit.position + Vector3.up * yOffset;
                if (Vector3.Distance(_player.position, spawnPos) > 50f)
                {
                    Instantiate(prefab, spawnPos, Quaternion.identity);
                    return;
                }
            }
            attempts--;
        } while (attempts > 0);
    }

    private void SpawnZombieAtRandomPatrolPoint()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0) return;

        int randomIndex = Random.Range(0, _patrolPoints.Length);
        Transform spawnPoint = _patrolPoints[randomIndex];

        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);

        if (zombie.TryGetComponent<BasicEnemyAI>(out var ai))
        {
            ai.SetPatrolPoints(_patrolPoints);
        }
    }

    private Vector3 RandomNavMeshLocation(float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range + _player.position;
        randomDirection.y = _player.position.y;
        return randomDirection;
    }
}
