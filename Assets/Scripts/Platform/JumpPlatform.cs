using UnityEngine;

// Jump platform: launches player upwards on contact
// Платформа-прыгун: подбрасывает игрока вверх при касании
public class JumpPlatform : MonoBehaviour
{
    [Header("JumpingPlatform")]
    public float jumpForce = 15f;    // Jump force // Сила прыжка 
    public float cooldownTime = 0.2f; // Cooldown between jumps // Время между активациями 
 
    private float lastJumpTime;     // Last jump timestamp // Последнее время прыжка 

    private AudioSource audioSource; // Jump sound // Звук прыжка 

    private void Awake()
    {
        // Get AudioSource component // Получаем компонент AudioSource 
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Set platform color to blue // Визуальная настройка: перекрас платформы в синий 
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.blue;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check collision with player // Проверка, что столкновение с игроком 
        if (collision.gameObject.CompareTag("PlayerRed") ||
            collision.gameObject.CompareTag("PlayerYellow") ||
            collision.gameObject.CompareTag("PlayerGreen") ||
            collision.gameObject.CompareTag("Player"))
        {
            // Check cooldown // Проверяем кулдаун 
            if (Time.time > lastJumpTime + cooldownTime)
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Reset vertical velocity // Сбрасываем вертикальную скорость перед прыжком 
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                    // Apply upward impulse // Применяем силу импульса вверх 
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                    lastJumpTime = Time.time;

                    // Play jump sound // Воспроизведение звука прыжка 
                    if (audioSource != null && audioSource.clip != null)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                    }

                    Debug.Log("Гравець підкинутий батутом. // Player launched by trampoline");
                }
            }
        }
    }
}