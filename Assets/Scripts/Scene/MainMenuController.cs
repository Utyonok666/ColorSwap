using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Manages main menu logic, scene transitions, and save data
// Управляет логикой главного меню, переходами между сценами и данными сохранений
public class MainMenuController : MonoBehaviour
{
    [Header("Audio / Звуки")]
    public AudioSource audioSource; 

    [Header("Buttons / Кнопки")]
    public GameObject quitButton;
    public GameObject continueButton;

    [Header("Panels & Text / Панели и Текст")]
    public GameObject settingsPanel;
    public CanvasGroup mainCanvasGroup;
    public TextMeshProUGUI recordText;

    [Header("Animation / Анимация")]
    public Animator settingsAnim;

    private void Start()
    {
        // Initial setup of UI elements
        // Начальная настройка элементов интерфейса
        if (quitButton != null) quitButton.SetActive(true);
        if (mainCanvasGroup != null) { mainCanvasGroup.gameObject.SetActive(true); mainCanvasGroup.alpha = 1f; }
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // Refresh save status and highscore on start
        // Проверяем статус сохранения и рекорд при старте
        CheckContinueStatus();
        UpdateRecordDisplay();
    }

    // Check save status every time the menu is enabled
    // Проверяем статус сохранения каждый раз, когда меню активируется
    private void OnEnable()
    {
        CheckContinueStatus();
    }

    public void PlayClickSound()
    {
        if (audioSource != null) audioSource.Play();
    }

    // Starts a fresh game and clears checkpoint data
    // Начинает новую игру и очищает данные контрольных точек
    public void NewGame()
    {
        PlayClickSound();
        
        // Clear current checkpoint coordinates (CPX, CPY, CPZ)
        // Очищаем текущие координаты чекпоинта (CPX, CPY, CPZ)
        PlayerPrefs.DeleteKey("CPX");
        PlayerPrefs.DeleteKey("CPY");
        PlayerPrefs.DeleteKey("CPZ");
        
        // Clear legacy save keys if they exist
        // Очищаем старые ключи сохранений, если они остались
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
        
        // Reset static checkpoint reference
        // Сбрасываем статическую переменную чекпоинта
        Checkpoint.LastCheckPointPos = Vector3.zero;

        PlayerPrefs.Save(); 

        // Load the first gameplay level (index 1)
        // Загружаем первый игровой уровень (индекс 1)
        SceneManager.LoadScene(1); 
    }

    // Continues the game from the last saved state
    // Продолжает игру с последнего сохраненного состояния
    public void ContinueGame()
    {
        PlayClickSound();
        // Load the main gameplay scene
        // Загружаем основную игровую сцену
        SceneManager.LoadScene(1); 
    }

    // Checks if a valid save exists to enable/disable the Continue button
    // Проверяет наличие сохранения для активации/деактивации кнопки "Продолжить"
    private void CheckContinueStatus() {
        // Verify if CPX coordinate exists in PlayerPrefs
        // Проверяем, существует ли координата CPX в PlayerPrefs
        bool hasSave = PlayerPrefs.HasKey("CPX");
        
        if (continueButton != null) 
        {
            continueButton.SetActive(hasSave);
        }
    }

    // Displays the player's highscore in meters
    // Отображает рекорд игрока в метрах
    public void UpdateRecordDisplay() {
        if (recordText != null) recordText.text = "Best: " + PlayerPrefs.GetFloat("Platformer_HighScore", 0f).ToString("F1") + "m";
    }

    // Opens settings panel and disables main menu interactivity
    // Открывает панель настроек и отключает взаимодействие с главным меню
    public void OpenSettings()
    {
        PlayClickSound();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (settingsAnim) settingsAnim.Play("Settings_Open");
            SetMenuInteractivity(false);
        }
    }

    // Closes settings with animation and restores menu interactivity
    // Закрывает настройки с анимацией и восстанавливает взаимодействие с меню
    public void BackToMenu()
    {
        PlayClickSound();
        if (settingsAnim) settingsAnim.Play("Settings_Close");
        SetMenuInteractivity(true);
        Invoke("CloseSettings", 0.5f);
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    // Toggles the interactivity and visual state of the main menu canvas
    // Переключает интерактивность и визуальное состояние основного холста меню
    private void SetMenuInteractivity(bool state) {
        if (mainCanvasGroup != null) { 
            mainCanvasGroup.interactable = state; 
            mainCanvasGroup.blocksRaycasts = state; 
            mainCanvasGroup.alpha = state ? 1f : 0.6f; 
        }
    }

    private void CloseSettings() => settingsPanel.SetActive(false);
}