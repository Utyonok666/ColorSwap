using UnityEngine;

// Provides a speed boost to the player when they enter the platform
// Даёт игроку ускорение при заходе на платформу
public class SpeedPlatform : MonoBehaviour
{
    [Header("Boost Settings")]
    public float boostForce = 25f;     // Amount of impulse to apply // Сила толчка
    public float boostDuration = 0.8f; // Duration without friction/slowdown // Время, когда торможение не работает

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for player tags
        // Проверяем по тегам игрока
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerRed") || 
            collision.CompareTag("PlayerYellow") || collision.CompareTag("PlayerGreen"))
        {
            Move playerMove = collision.GetComponent<Move>();
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (playerMove != null && rb != null)
            {
                // 1. Apply instant velocity along platform's forward direction
                // 1. Мгновенно задаём скорость в направлении платформы
                rb.linearVelocity = new Vector2(transform.right.x * boostForce, rb.linearVelocity.y);

                // 2. Disable friction/slowdown in Move script for boost duration
                // 2. Отключаем трение/замедление в скрипте Move на время буста
                playerMove.ApplyBoost(boostDuration);

                Debug.Log("Boost Activated for: " + collision.tag);
            }
        }
    }
}