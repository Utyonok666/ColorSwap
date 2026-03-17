using UnityEngine;
using UnityEngine.SceneManagement;

// Game manager singleton (Менеджер игры, синглтон)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance (Ссылка на синглтон)

    [Header("References (Ссылки)")]
    public GameObject player; // Player reference (Ссылка на игрока)
    public Move playerMove; // Player movement script (Скрипт движения игрока)
    public SwapColor swapColorScript; // Player color swap script (Скрипт смены цвета игрока)

    void Awake()
    {
        // Singleton setup (Настройка синглтона)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Reset player to last checkpoint (Сброс игрока к последнему чекпоинту)
    public void RestartPlayer()
    {
        // Find player by tag if reference is null (Находим игрока по тегу, если ссылка пуста)
        if (player == null) 
            player = GameObject.FindGameObjectWithTag("Player") ?? 
                     GameObject.FindGameObjectWithTag("PlayerRed") ??
                     GameObject.FindGameObjectWithTag("PlayerYellow") ??
                     GameObject.FindGameObjectWithTag("PlayerGreen");

        if (player != null)
        {
            // 1. Move to last checkpoint (Перемещаем на чекпоинт)
            player.transform.position = Checkpoint.LastCheckPointPos;

            // 2. Reset physics (Сбрасываем физику)
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 3. Update platforms according to player color (Обновляем мир под цвет игрока)
            if (PlatformManager.Instance != null)
                PlatformManager.Instance.NotifyColorChange(player.tag);

            // 4. Enable input if disabled (Включаем управление, если было отключено)
            if (playerMove != null) playerMove.SetInputState(true);
        }

        // Reset camera position (Сброс камеры)
        if (Camera.main != null)
        {
            var cc = Camera.main.GetComponent<CameraControl>();
            if (cc != null) cc.TeleportToTarget();
        }
    }

    // Update platforms when player physics triggers (Обновление платформ при взаимодействии игрока)
    public void UpdatePhysicsForPlayer(Collider2D playerCol)
    {
        if (PlatformManager.Instance != null && playerCol != null)
        {
            PlatformManager.Instance.NotifyColorChange(playerCol.tag);
        }
    }

    // Restart the entire scene (Перезагрузка сцены)
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}