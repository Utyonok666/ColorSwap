using UnityEngine;

public class SpeedPlatform : MonoBehaviour
{
    [Header("Настройки буста")]
    public float boostForce = 25f;    // Сила толчка
    public float boostDuration = 0.8f; // Сколько времени НЕ будет работать торможение

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем игрока по всем твоим тегам
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerRed") || 
            collision.CompareTag("PlayerYellow") || collision.CompareTag("PlayerGreen"))
        {
            Move playerMove = collision.GetComponent<Move>();
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (playerMove != null && rb != null)
            {
                // 1. Мгновенно даем скорость в направлении стрелки платформы (вправо)
                // Используем velocity (или linearVelocity, если Unity 2023+)
                rb.linearVelocity = new Vector2(transform.right.x * boostForce, rb.linearVelocity.y);
                
                // 2. САМОЕ ВАЖНОЕ: отключаем трение в скрипте Move на время буста
                playerMove.ApplyBoost(boostDuration);
                
                Debug.Log("Boost Activated for: " + collision.tag);
            }
        }
    }
}