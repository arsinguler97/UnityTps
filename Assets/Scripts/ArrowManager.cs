using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private int maxArrowCount = 10;

    private Queue<GameObject> _arrows = new Queue<GameObject>();

    public static ArrowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterArrow(GameObject arrow)
    {
        _arrows.Enqueue(arrow);

        if (_arrows.Count > maxArrowCount)
        {
            GameObject oldestArrow = _arrows.Dequeue();
            if (oldestArrow != null)
            {
                Destroy(oldestArrow);
            }
        }
    }
}