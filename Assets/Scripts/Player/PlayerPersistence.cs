using UnityEngine;
using UnityEngine.SceneManagement;

// Handles player persistence between scenes / Управление сохранением позиции игрока
public class PlayerPersistence : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // On level start, check if checkpoint exists in memory
        // При старте уровня проверяем: есть ли сохранённый чекпоинт в памяти
        if (PlayerPrefs.HasKey("CPX"))
        {
            float x = PlayerPrefs.GetFloat("CPX");
            float y = PlayerPrefs.GetFloat("CPY");
            float z = PlayerPrefs.GetFloat("CPZ");

            // Temporarily disable physics to prevent getting stuck in colliders
            // Временно отключаем физику, чтобы игрок не застрял в коллайдерах
            if (rb) rb.simulated = false;
            transform.position = new Vector3(x, y, z);
            if (rb) rb.simulated = true;

            Debug.Log("Player loaded at checkpoint / Игрок загружен на чекпоинте.");
        }
    }

    void Update()
    {
        // Press R to return to last checkpoint
        // Клавиша R возвращает игрока на последний чекпоинт
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartToCheckpoint();
        }
    }

    public void RestartToCheckpoint()
    {
        if (PlayerPrefs.HasKey("CPX"))
        {
            float x = PlayerPrefs.GetFloat("CPX");
            float y = PlayerPrefs.GetFloat("CPY");
            float z = PlayerPrefs.GetFloat("CPZ");

            // Reset velocity to prevent "flying" on teleport
            // Сбрасываем скорость, чтобы игрок не вылетел после телепорта
            if (rb) rb.linearVelocity = Vector2.zero;
            transform.position = new Vector3(x, y, z);

            Debug.Log("Quick return to checkpoint (R key) / Быстрый возврат на чекпоинт (клавиша R)");
        }
        else
        {
            // If no checkpoint exists yet, restart the level
            // Если чекпоинт ещё не пройден — перезапускаем уровень
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}