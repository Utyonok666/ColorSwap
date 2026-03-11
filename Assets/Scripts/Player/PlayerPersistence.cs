using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // При старте уровня проверяем: есть ли сохраненный чекпоинт в памяти?
        if (PlayerPrefs.HasKey("CPX"))
        {
            float x = PlayerPrefs.GetFloat("CPX");
            float y = PlayerPrefs.GetFloat("CPY");
            float z = PlayerPrefs.GetFloat("CPZ");
            
            // Отключаем физику на миг для телепортации (чтобы не застрять в текстурах)
            if (rb) rb.simulated = false;
            transform.position = new Vector3(x, y, z);
            if (rb) rb.simulated = true;
            
            Debug.Log("Игрок загружен на чекпоинте.");
        }
    }

    void Update()
    {
        // Возврат на чекпоинт по кнопке R
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
            
            if (rb) rb.linearVelocity = Vector2.zero; // Сбрасываем скорость, чтобы не вылететь пулей
            transform.position = new Vector3(x, y, z);
            
            Debug.Log("Быстрый возврат на чекпоинт (клавиша R)");
        }
        else
        {
            // Если игрок нажал R, но еще не касался чекпоинта — рестарт уровня
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}