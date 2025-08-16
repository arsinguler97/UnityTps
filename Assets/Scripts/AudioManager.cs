using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        volumeSlider.value = GameManager.Instance.MusicVolume;
        AudioListener.volume = volumeSlider.value;
    }

    private void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;
        GameManager.Instance.MusicVolume = volume;
    }
}