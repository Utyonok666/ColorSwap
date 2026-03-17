using UnityEngine;
using UnityEngine.SceneManagement;

// Manages game pause state, UI panels, and cursor visibility
// Управляет состоянием паузы, панелями интерфейса и видимостью курсора
public class PauseManager : MonoBehaviour
{
    [Header("Panels / Панели")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject confirmPanel;

    private bool isPaused = false;

    void Start()
    {
        // Guaranteed reset of state when the scene starts
        // Гарантированный сброс состояния при старте сцены
        Time.timeScale = 1f;
        isPaused = false;

        // Ensure all UI panels are hidden
        // Убеждаемся, что все панели интерфейса скрыты
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmPanel.SetActive(false);

        // Hide and lock cursor for gameplay
        // Скрываем и блокируем курсор для игрового процесса
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Toggle pause state with the Escape key
        // Переключение состояния паузы клавишей Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    // Stops time and shows the pause menu
    // Останавливает время и показывает меню паузы
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        confirmPanel.SetActive(false);

        // Show cursor for menu navigation
        // Показываем курсор для навигации по меню
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Resumes time and hides all pause UI
    // Возобновляет время и скрывает весь интерфейс паузы
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmPanel.SetActive(false);

        // Hide cursor back for gameplay
        // Снова скрываем курсор для игры
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // Resets time scale and returns to the main menu
    // Сбрасывает масштаб времени и возвращает в главное меню
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}