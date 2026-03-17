using UnityEngine;
using UnityEngine.SceneManagement;

// Handles player restart and level reload
// Управляет перезапуском игрока и перезагрузкой уровня
public class Restart : MonoBehaviour
{
    void Update()
    {
        // Restart player to last checkpoint (Key R)
        // Перезапуск игрока на последний чекпоинт (Кнопка R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            DoRestartPlayer();
        }

        // Reload entire level (Key L)
        // Полная перезагрузка уровня (Кнопка L)
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartScene();
            }
        }
    }

    // Called from UI button to restart player
    // Вызывается кнопкой UI для перезапуска игрока
    public void OnRestartButtonClick()
    {
        DoRestartPlayer();
    }

    // Core restart logic separated to avoid code duplication
    // Основная логика перезапуска вынесена для избежания дублирования кода
    private void DoRestartPlayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartPlayer();

            // --- Решение для сброса скорости игрока ---
            // Find the player object (by tag "Player")
            // Находим объект игрока (по тегу "Player")
            GameObject player = GameObject.FindGameObjectWithTag("Player"); 
            
            // If using multiple tags (PlayerRed, PlayerYellow, etc.)
            // Если используются разные теги (PlayerRed, PlayerYellow и т.д.)
            if (player == null) player = GameObject.Find("Player"); // Fallback by name (резерв по имени)

            if (player != null)
            {
                // Reset player boost/speed after restart
                // Сбрасываем ускорение/скорость игрока после рестарта
                Move moveScript = player.GetComponent<Move>();
                if (moveScript != null)
                {
                    moveScript.ResetBoost(); 
                    Debug.Log("Restart: Boost reset via R"); // Debug log (Отладка)
                }
            }
        }
    }
}