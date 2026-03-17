using UnityEngine;

// Handles destroying or respawning a moving platform when collided
// Скрипт для уничтожения или респавна движущейся платформы при столкновении
public class PlatformDestroyer : MonoBehaviour
{
    [Header("Which platform to destroy?")]
    [Tooltip("Drag the platform object from hierarchy here")]
    public MovingPlatformSimple targetPlatform; // Platform to affect // Целевая платформа

    [Header("Effect settings")]
    public bool playEffect = true;             // Whether to play sound/particles // Воспроизводить эффект
    public AudioClip destroySound;             // Optional destroy sound // Звук "вжух" или "бдыщ"

    private AudioSource _audioSource;          // Audio source for effects // Аудио источник для эффектов

    private void Awake()
    {
        // Ensure AudioSource exists if we want effects
        // Проверяем наличие AudioSource, если нужны эффекты
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null && playEffect)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if we have a target platform
        // Проверяем, есть ли цель для уничтожения
        if (targetPlatform == null) return;

        // 2. Check if the collided object is our target platform
        // Сравниваем вошедший объект с целевой платформой
        if (other.gameObject == targetPlatform.gameObject)
        {
            // Call respawn method from MovingPlatformSimple
            // Вызываем респавн платформы
            targetPlatform.InstantRespawn();

            Debug.Log("<color=red>Target platform destroyed!</color>");

            if (playEffect) PlayDestroyEffect();
        }
        else
        {
            // Log ignored objects for testing
            // Сообщаем о проигнорированных объектах (для теста)
            Debug.Log("Object " + other.name + " ignored by PlatformDestroyer.");
        }
    }

    private void PlayDestroyEffect()
    {
        // Play destroy sound if assigned
        // Проигрываем звук, если назначен
        if (_audioSource != null && destroySound != null)
        {
            _audioSource.PlayOneShot(destroySound);
        }

        // You can add particle effects here if needed
        // Можно добавить частицы (Particles) здесь
    }
}