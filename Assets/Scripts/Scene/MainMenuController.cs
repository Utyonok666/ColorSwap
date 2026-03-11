using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Звуки")]
    public AudioSource audioSource; 

    [Header("Кнопки")]
    public GameObject quitButton;
    public GameObject continueButton;

    [Header("Панели и Текст")]
    public GameObject settingsPanel;
    public CanvasGroup mainCanvasGroup;
    public TextMeshProUGUI recordText;

    [Header("Анимация")]
    public Animator settingsAnim;

    private void Start()
    {
        if (quitButton != null) quitButton.SetActive(true);
        if (mainCanvasGroup != null) { mainCanvasGroup.gameObject.SetActive(true); mainCanvasGroup.alpha = 1f; }
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // Проверяем статус при старте
        CheckContinueStatus();
        UpdateRecordDisplay();
    }

    // Добавим OnEnable, чтобы кнопка проверялась каждый раз, когда мы попадаем в меню
    private void OnEnable()
    {
        CheckContinueStatus();
    }

    public void PlayClickSound()
    {
        if (audioSource != null) audioSource.Play();
    }

    public void NewGame()
    {
        PlayClickSound();
        
        // Очищаем ключи (теперь используем CPX/CPY/CPZ как в твоем скрипте Checkpoint)
        PlayerPrefs.DeleteKey("CPX");
        PlayerPrefs.DeleteKey("CPY");
        PlayerPrefs.DeleteKey("CPZ");
        
        // Очищаем старые ключи, если они остались от прошлых версий
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
        
        // Сбрасываем статическую переменную
        Checkpoint.LastCheckPointPos = Vector3.zero;

        PlayerPrefs.Save(); 

        // Загружаем первый уровень (индекс 1)
        SceneManager.LoadScene(1); 
    }

    public void ContinueGame()
    {
        PlayClickSound();
        // Загружаем уровень (если сохраняешь индекс) или просто сцену игры
        // Если уровней много, здесь лучше использовать PlayerPrefs.GetInt("SavedLevel", 1)
        SceneManager.LoadScene(1); 
    }

    // --- Остальные методы (Settings, Quit и т.д.) оставляем как есть ---

    private void CheckContinueStatus() {
        // ВАЖНО: Проверяем именно ключ "CPX", который создает твой скрипт Checkpoint
        bool hasSave = PlayerPrefs.HasKey("CPX");
        
        if (continueButton != null) 
        {
            continueButton.SetActive(hasSave);
        }
    }

    public void UpdateRecordDisplay() {
        if (recordText != null) recordText.text = "Best: " + PlayerPrefs.GetFloat("Platformer_HighScore", 0f).ToString("F1") + "m";
    }

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

    private void SetMenuInteractivity(bool state) {
        if (mainCanvasGroup != null) { mainCanvasGroup.interactable = state; mainCanvasGroup.blocksRaycasts = state; mainCanvasGroup.alpha = state ? 1f : 0.6f; }
    }

    private void CloseSettings() => settingsPanel.SetActive(false);
}