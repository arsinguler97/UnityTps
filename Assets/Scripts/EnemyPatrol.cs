using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 _startPosition;
    private int _direction = 1;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * (_direction * speed * Time.deltaTime));

        float distanceFromStart = Vector3.Distance(transform.position, _startPosition);

        if (distanceFromStart >= moveDistance)
        {
            _direction *= -1;
        }
    }
}