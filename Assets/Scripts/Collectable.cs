using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    private Tween _moveTween;
    private Tween _rotateTween;

    private void Start()
    {
        _moveTween = transform.DOMoveY(transform.position.y + 0.5f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        _rotateTween = transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _moveTween.Kill();
            _rotateTween.Kill();

            Destroy(gameObject);
        }
    }
}