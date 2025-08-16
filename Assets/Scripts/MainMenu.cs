using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject volumeSlider;

    private void Start()
    {
        volumeSlider.SetActive(false);
    }

    public void PlayGame()
    {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void ToggleVolumeSlider()
    {
        volumeSlider.SetActive(!volumeSlider.activeSelf);
    }
}
