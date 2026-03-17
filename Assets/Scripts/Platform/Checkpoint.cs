using UnityEngine;

// Handles saving and loading the last checkpoint position
// Управляет сохранением и загрузкой последней позиции чекпоинта
public class Checkpoint : MonoBehaviour
{
    public static Vector3 LastCheckPointPos = Vector3.zero; 
    // Last checkpoint position in the game // Последняя позиция чекпоинта в игре

    private void Awake()
    {
        // Load saved checkpoint from PlayerPrefs if it exists
        // Загружаем сохраненный чекпоинт из PlayerPrefs, если есть
        if (PlayerPrefs.HasKey("CPX"))
        {
            LastCheckPointPos = new Vector3(
                PlayerPrefs.GetFloat("CPX"),
                PlayerPrefs.GetFloat("CPY"),
                PlayerPrefs.GetFloat("CPZ")
            );
        }
        else
        {
            LastCheckPointPos = Vector3.zero; // Default start position // Стартовая позиция по умолчанию
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collider is the player (any color variant)
        // Проверяем, является ли объект игроком (любой цвет)
        if (other.CompareTag("Player") || other.CompareTag("PlayerRed") || 
            other.CompareTag("PlayerYellow") || other.CompareTag("PlayerGreen"))
        {
            LastCheckPointPos = transform.position; // Update last checkpoint // Обновляем последний чекпоинт

            // Save checkpoint to PlayerPrefs
            // Сохраняем чекпоинт
            PlayerPrefs.SetFloat("CPX", transform.position.x);
            PlayerPrefs.SetFloat("CPY", transform.position.y);
            PlayerPrefs.SetFloat("CPZ", transform.position.z);
            PlayerPrefs.Save(); 

            Debug.Log("Чекпоинт сохранен!"); // Debug log // Лог для отладки
        }
    }
}