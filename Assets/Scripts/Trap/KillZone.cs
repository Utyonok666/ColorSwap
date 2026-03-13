using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что это игрок (по началу тега)
        if (other.tag.StartsWith("Player")) Respawn(other.gameObject);
    }

    void Respawn(GameObject player)
    {
        // 1. Перемещаем игрока на чекпоинт
        player.transform.position = Checkpoint.LastCheckPointPos;

        // 2. КЛЮЧЕВОЙ МОМЕНТ: Находим скрипт Move и вызываем полную очистку буста и физики
        Move playerMove = player.GetComponent<Move>();
        if (playerMove != null)
        {
            playerMove.ResetBoost(); 
        }

        // 3. Твоя остальная логика (цвета, камера)
        if (PlatformManager.Instance != null)
            PlatformManager.Instance.NotifyColorChange(player.tag);

        if (Camera.main != null)
        {
            var cc = Camera.main.GetComponent<CameraControl>();
            if (cc != null) cc.TeleportToTarget();
        }
    }
}