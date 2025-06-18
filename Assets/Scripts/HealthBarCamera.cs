using UnityEngine;

public class HealthBarCamera : MonoBehaviour
{
    private Camera _mainCam;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCam == null) return;

        transform.LookAt(transform.position + _mainCam.transform.forward);
    }
}