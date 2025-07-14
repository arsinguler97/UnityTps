using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "TriggerOpen";
    [SerializeField] private string closeTrigger = "TriggerClose";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool _isPlayerNearby = false;
    private bool _isOpen = false;

    private void Update()
    {
        if (_isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            if (_isOpen)
            {
                doorAnimator.ResetTrigger(openTrigger);
                doorAnimator.SetTrigger(closeTrigger);
            }
            else
            {
                doorAnimator.ResetTrigger(closeTrigger);
                doorAnimator.SetTrigger(openTrigger);
            }

            _isOpen = !_isOpen;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _isPlayerNearby = false;
    }
}