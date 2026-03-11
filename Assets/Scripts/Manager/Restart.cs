using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    void Update()
    {
        // Перезагрузка игрока на чекпоинт (Кнопка R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartPlayer();
            }
        }

        // Полная перезагрузка уровня (Кнопка L или любая другая на выбор)
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartScene();
            }
        }
    }

    // Если этот скрипт висит на кнопке UI
    public void OnRestartButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartPlayer();
        }
    }
}