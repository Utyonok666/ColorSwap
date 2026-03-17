using UnityEngine;

// Jelly zone: slows player and plays sound while inside
// Зона желе: замедляет игрока и проигрывает звук при нахождении внутри
public class JellyZone : MonoBehaviour
{
    [Header("Настройки желе")]
    public float slowFactor = 0.5f; // Player slowdown factor // Множитель замедления игрока 
    public float exitBoost = 1.2f;  // Speed boost on exit // Множитель при выходе из зоны 

    private AudioSource audioSource; // Jelly audio // Звук желе 

    private void Awake()
    {
        // Get AudioSource component // Берем компонент AudioSource на объекте 
        audioSource = GetComponent<AudioSource>();

        // Loop audio // Настройка: звук должен зацикливаться 
        if (audioSource != null)
        {
            audioSource.loop = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if object is a player // Проверка, что объект — игрок 
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerYellow") ||
            other.CompareTag("PlayerGreen") || other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Move move = other.GetComponent<Move>();

            if (rb != null && move != null)
            {
                // Apply slowdown // Замедляем движение игрока 
                move.SetMovementModifiers(slowFactor, 1f);
                rb.linearVelocity *= slowFactor;

                // Play audio if not playing // Включаем звук, если он еще не играет 
                if (audioSource != null && !audioSource.isPlaying)
                    audioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Player exits zone // Проверка выхода игрока из зоны 
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerYellow") ||
            other.CompareTag("PlayerGreen") || other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Move move = other.GetComponent<Move>();

            if (rb != null && move != null)
            {
                // Reset movement // Восстанавливаем нормальное движение 
                move.SetMovementModifiers(1f, 1f);
                rb.linearVelocity *= exitBoost;

                // Stop audio // Останавливаем звук 
                if (audioSource != null && audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
    }
}