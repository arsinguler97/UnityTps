using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject volumeSlider;

    public static bool GameIsPaused { get; private set; }

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        volumeSlider.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }
    
    private void OnEnable()
    {
        GameIsPaused = false;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        volumeSlider.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        volumeSlider.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToggleVolumeSlider()
    {
        volumeSlider.SetActive(!volumeSlider.activeSelf);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("MainMenu");
    }
}