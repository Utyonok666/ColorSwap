using UnityEngine;
using System.Collections;

// Teleports the player to a target location when they enter the trigger
// Телепортирует игрока в заданную точку при входе в триггер
public class Teleporter : MonoBehaviour
{
    public Transform exitPoint;      // Destination of teleport // Точка выхода
    public float teleportCooldown = 0.5f; // Time before teleport can be used again // Время перезарядки телепорта
    private static bool _isTeleporting = false; // Prevents multiple teleports at once // Не позволяет телепортироваться несколько раз сразу

    private AudioSource audioSource; // Optional sound effect // Опциональный аудиокомпонент

    private void Awake()
    {
        // Get the AudioSource component if attached
        // Берем компонент AudioSource, если он есть
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTeleporting) return; // Ignore if teleport is already happening // Игнорируем, если телепорт уже идет

        // Check player tags
        // Проверяем теги игрока
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerYellow") || 
            other.CompareTag("PlayerGreen") || other.CompareTag("Player"))
        {
            if (exitPoint != null)
            {
                StartCoroutine(DoTeleport(other.transform)); // Start teleport coroutine // Запускаем корутину телепорта
            }
        }
    }

    private IEnumerator DoTeleport(Transform player)
    {
        _isTeleporting = true; // Lock teleport // Блокируем телепорт

        // Play sound if available
        // Проигрываем звук, если есть
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // Move player to exit point, keeping Z = 0 for 2D
        // Перемещаем игрока на точку выхода, фиксируя Z = 0 для 2D
        player.position = new Vector3(exitPoint.position.x, exitPoint.position.y, 0f);

        // Update camera position if main camera has CameraControl
        // Обновляем позицию камеры, если у основной камеры есть CameraControl
        if (Camera.main != null)
        {
            var cam = Camera.main.GetComponent<CameraControl>();
            if (cam != null) cam.TeleportToTarget();
        }

        // Wait for cooldown before allowing next teleport
        // Ждем перезарядку перед следующим телепортом
        yield return new WaitForSeconds(teleportCooldown);

        _isTeleporting = false; // Unlock teleport // Разблокируем телепорт
    }
}