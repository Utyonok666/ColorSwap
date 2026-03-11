using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Ссылки")]
    public GameObject player;
    public Move playerMove;
    public SwapColor swapColorScript;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Тот самый метод, который ищет Restart.cs
    public void RestartPlayer()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player") ?? 
                                     GameObject.FindGameObjectWithTag("PlayerRed") ??
                                     GameObject.FindGameObjectWithTag("PlayerYellow") ??
                                     GameObject.FindGameObjectWithTag("PlayerGreen");

        if (player != null)
        {
            // 1. Возвращаем на чекпоинт
            player.transform.position = Checkpoint.LastCheckPointPos;

            // 2. Сбрасываем физику
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 3. Обновляем мир под цвет игрока
            if (PlatformManager.Instance != null)
                PlatformManager.Instance.NotifyColorChange(player.tag);

            // 4. Включаем управление, если оно было выключено
            if (playerMove != null) playerMove.SetInputState(true);
        }
        
        // Сбрасываем камеру
        if (Camera.main != null)
        {
            var cc = Camera.main.GetComponent<CameraControl>();
            if (cc != null) cc.TeleportToTarget();
        }
    }

    public void UpdatePhysicsForPlayer(Collider2D playerCol)
    {
        if (PlatformManager.Instance != null && playerCol != null)
        {
            PlatformManager.Instance.NotifyColorChange(playerCol.tag);
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}