using UnityEngine;

public class PlatformDestroyer : MonoBehaviour
{
    [Header("Какую платформу уничтожать?")]
    [Tooltip("Перетяни сюда объект платформы из иерархии")]
    public MovingPlatformSimple targetPlatform; 

    [Header("Настройки эффектов")]
    public bool playEffect = true;
    public AudioClip destroySound; // Сюда можно кинуть звук "вжух" или "бдыщ"

    private AudioSource _audioSource;

    private void Awake()
    {
        // Добавляем AudioSource сами, если его нет
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null && playEffect)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Проверяем, есть ли у нас вообще цель
        if (targetPlatform == null) return;

        // 2. Сравниваем вошедший объект с нашей целевой платформой
        if (other.gameObject == targetPlatform.gameObject)
        {
            // Вызываем метод респавна, который мы прописали в MovingPlatformSimple
            targetPlatform.InstantRespawn();

            Debug.Log("<color=red>Целевая платформа уничтожена!</color>");
            
            if (playEffect) PlayDestroyEffect();
        }
        else
        {
            // Сообщение в консоль для теста (можно потом удалить)
            Debug.Log("Объект " + other.name + " проигнорирован уничтожителем.");
        }
    }

    private void PlayDestroyEffect()
    {
        // Проигрываем звук, если он назначен
        if (_audioSource != null && destroySound != null)
        {
            _audioSource.PlayOneShot(destroySound);
        }

        // Если захочешь добавить частицы (Particles), создавай их здесь
    }
}