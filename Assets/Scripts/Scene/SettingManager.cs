using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer myMixer;
    public Slider musicSlider, sfxSlider;

    [Header("Resolution Settings")]
    public TMP_Dropdown resDropdown;
    public GameObject confirmPanel;
    public TextMeshProUGUI timerText;

    private Resolution oldRes;
    private Coroutine timerCoroutine;

    void Start()
    {
        // Сначала настраиваем UI, потом применяем звук
        SetupResolutions();
        InitAudioSettings(); 
    }

    // --- БЛОК ЗВУКА (УЛУЧШЕННЫЙ) ---

    void InitAudioSettings()
    {
        if (myMixer == null) return;

        // Загружаем значения (по умолчанию 1, т.е. 100% громкости, если сохранений нет)
        float musicVal = PlayerPrefs.GetFloat("SavedMusicVol", 1f);
        float sfxVal = PlayerPrefs.GetFloat("SavedSfxVol", 1f);

        // Ставим слайдеры в нужное положение
        if (musicSlider != null) musicSlider.value = musicVal;
        if (sfxSlider != null) sfxSlider.value = sfxVal;

        // Применяем звук сразу
        SetMusic(musicVal);
        SetSfx(sfxVal);
    }

    public void SetMusic(float sliderValue)
    {
        if (myMixer != null)
        {
            // Формула для плавного звука: Log10 превращает 0.0001-1.0 в -80dB-0dB
            // Mathf.Clamp нужен, чтобы логарифм не выдал -бесконечность при 0
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
    
    // Метод для кнопки "Save" (если нужна принудительная запись, хотя PlayerPrefs пишет сам при выходе)
    public void SavePreferences()
    {
        PlayerPrefs.Save();
    }

    // --- БЛОК ГРАФИКИ (БЕЗ ИЗМЕНЕНИЙ, ВСЁ ОК) ---

    void SetupResolutions()
    {
        if (resDropdown == null) return;
        
        resDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;

        Resolution[] resolutions = Screen.resolutions;

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

    public void ApplySettings()
    {
        oldRes = Screen.currentResolution;
        Resolution newRes = Screen.resolutions[resDropdown.value];
        Screen.SetResolution(newRes.width, newRes.height, FullScreenMode.FullScreenWindow);

        if (confirmPanel != null) confirmPanel.SetActive(true);
        
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(ConfirmationTimer());
    }

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

    public void ConfirmSettings()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void RevertResolution()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        Screen.SetResolution(oldRes.width, oldRes.height, FullScreenMode.FullScreenWindow);
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }
}