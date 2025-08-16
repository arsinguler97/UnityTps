using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GroundCheck _groundCheck;

    private void Update()
    {
        if (IsMoving() && _groundCheck.IsGrounded)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }
    }

    private bool IsMoving()
    {
        return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
    }
}