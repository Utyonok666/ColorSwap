using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

// Manages game settings: Audio volume, Screen resolution, and Preferences saving
// Управляет настройками игры: громкостью звука, разрешением экрана и сохранением предпочтений
public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings / Настройки звука")]
    public AudioMixer myMixer;
    public Slider musicSlider, sfxSlider;

    [Header("Resolution Settings / Настройки разрешения")]
    public TMP_Dropdown resDropdown;
    public GameObject confirmPanel;
    public TextMeshProUGUI timerText;

    private Resolution oldRes;
    private Coroutine timerCoroutine;

    void Start()
    {
        // Setup UI first, then apply audio settings
        // Сначала настраиваем UI, потом применяем настройки звука
        SetupResolutions();
        InitAudioSettings(); 
    }

    // --- AUDIO BLOCK / БЛОК ЗВУКА ---

    void InitAudioSettings()
    {
        if (myMixer == null) return;

        // Load saved volumes (default to 1.0 / 100% if no save exists)
        // Загружаем значения громкости (по умолчанию 1.0, если сохранений нет)
        float musicVal = PlayerPrefs.GetFloat("SavedMusicVol", 1f);
        float sfxVal = PlayerPrefs.GetFloat("SavedSfxVol", 1f);

        // Set sliders to the correct positions
        // Устанавливаем слайдеры в нужное положение
        if (musicSlider != null) musicSlider.value = musicVal;
        if (sfxSlider != null) sfxSlider.value = sfxVal;

        // Apply audio levels immediately
        // Применяем уровни звука сразу
        SetMusic(musicVal);
        SetSfx(sfxVal);
    }

    public void SetMusic(float sliderValue)
    {
        if (myMixer != null)
        {
            // Logarithmic formula to convert 0.0001-1.0 slider value to -80dB-0dB
            // Логарифмическая формула для перевода значений слайдера в децибелы
            float volume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
            
            myMixer.SetFloat("MusicVol", volume);
            PlayerPrefs.SetFloat("SavedMusicVol", sliderValue); 
        }
    }

    public void SetSfx(float sliderValue)
    {
        if (myMixer != null)
        {
            float volume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
            
            myMixer.SetFloat("SfxVol", volume);
            PlayerPrefs.SetFloat("SavedSfxVol", sliderValue);
        }
    }
    
    // Forces PlayerPrefs to write to disk
    // Принудительное сохранение настроек на диск
    public void SavePreferences()
    {
        PlayerPrefs.Save();
    }

    // --- GRAPHICS BLOCK / БЛОК ГРАФИКИ ---

    void SetupResolutions()
    {
        if (resDropdown == null) return;
        
        resDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;

        Resolution[] resolutions = Screen.resolutions;

        // Populate dropdown with available screen resolutions
        // Заполняем выпадающий список доступными разрешениями экрана
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            
            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resDropdown.AddOptions(options);
        resDropdown.value = currentResIndex;
        resDropdown.RefreshShownValue();
    }

    // Applies selected resolution and shows confirmation dialog
    // Применяет выбранное разрешение и показывает окно подтверждения
    public void ApplySettings()
    {
        oldRes = Screen.currentResolution;
        Resolution newRes = Screen.resolutions[resDropdown.value];
        Screen.SetResolution(newRes.width, newRes.height, FullScreenMode.FullScreenWindow);

        if (confirmPanel != null) confirmPanel.SetActive(true);
        
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(ConfirmationTimer());
    }

    // Reverts resolution automatically if not confirmed within 15 seconds
    // Автоматический возврат разрешения, если оно не подтверждено за 15 секунд
    System.Collections.IEnumerator ConfirmationTimer()
    {
        float timer = 15f;
        while (timer > 0)
        {
            if (timerText != null) timerText.text = "Сохранить? " + Mathf.Ceil(timer);
            yield return new WaitForSecondsRealtime(1f); 
            timer--;
        }
        RevertResolution();
    }

    // Keeps the new resolution and saves data
    // Сохраняет новое разрешение и данные игрока
    public void ConfirmSettings()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    // Reverts to the previous resolution
    // Возвращает предыдущее разрешение экрана
    public void RevertResolution()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        Screen.SetResolution(oldRes.width, oldRes.height, FullScreenMode.FullScreenWindow);
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }
}