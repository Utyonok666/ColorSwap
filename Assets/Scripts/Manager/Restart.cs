using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    void Update()
    {
        // Перезагрузка игрока на чекпоинт (Кнопка R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            DoRestartPlayer();
        }

        // Полная перезагрузка уровня (Кнопка L)
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartScene();
            }
        }
    }

    public void OnRestartButtonClick()
    {
        DoRestartPlayer();
    }

    // Вынес логику в отдельный метод, чтобы не дублировать код
    private void DoRestartPlayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartPlayer();

            // --- ВОТ ТУТ РЕШЕНИЕ ПРОБЛЕМЫ ---
            // Ищем игрока (по тегу, как в KillZone)
            GameObject player = GameObject.FindGameObjectWithTag("Player"); 
            
            // Если у тебя разные теги (PlayerRed и т.д.), лучше искать так:
            if (player == null) player = GameObject.Find("Player"); // Если объект называется Player

            if (player != null)
            {
                Move moveScript = player.GetComponent<Move>();
                if (moveScript != null)
                {
                    moveScript.ResetBoost(); // Сбрасываем таймер и скорость
                    Debug.Log("Рестарт: Скорость сброшена через R");
                }
            }
        }
    }
}