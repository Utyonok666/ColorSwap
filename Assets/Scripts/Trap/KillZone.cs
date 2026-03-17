using UnityEngine;

// Handles player "death" and respawning when entering a danger zone (e.g., pits, spikes)
// Обрабатывает «смерть» и возврат игрока на точку сохранения при попадании в опасную зону
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player (by tag prefix)
        // Проверяем, что это игрок (по началу тега)
        if (other.tag.StartsWith("Player")) Respawn(other.gameObject);
    }

    // Handles the teleportation and state reset of the player
    // Управляет телепортацией и сбросом состояния игрока
    void Respawn(GameObject player)
    {
        // 1. Move player to the last recorded checkpoint position
        // 1. Перемещаем игрока на позицию последней контрольной точки
        player.transform.position = Checkpoint.LastCheckPointPos;

        // 2. IMPORTANT: Access the Move script to clear boost and physics velocity
        // 2. ВАЖНО: Обращаемся к скрипту Move для очистки ускорения и физики
        Move playerMove = player.GetComponent<Move>();
        if (playerMove != null)
        {
            playerMove.ResetBoost(); 
        }

        // 3. Update world state: platforms and camera
        // 3. Обновляем состояние мира: платформы и камеру

        // Notify managers about the player's current color after respawn
        // Уведомляем менеджеры о текущем цвете игрока после респауна
        if (PlatformManager.Instance != null)
            PlatformManager.Instance.NotifyColorChange(player.tag);

        // Instantly move the camera to the player to avoid smooth-follow delay
        // Мгновенно перемещаем камеру к игроку, чтобы избежать задержки следования
        if (Camera.main != null)
        {
            var cc = Camera.main.GetComponent<CameraControl>();
            if (cc != null) cc.TeleportToTarget();
        }
    }
}