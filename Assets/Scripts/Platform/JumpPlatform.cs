using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [Header("JumpingPlatform")]
    public float jumpForce = 15f; 
    public float cooldownTime = 0.2f;

    private float lastJumpTime;
    
    // Додаємо змінну для звуку
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.blue;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerRed") ||
            collision.gameObject.CompareTag("PlayerYellow") ||
            collision.gameObject.CompareTag("PlayerGreen") ||
            collision.gameObject.CompareTag("Player")) // Додав звичайний тег Player про всяк випадок
        {
            if (Time.time > lastJumpTime + cooldownTime)
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    
                    lastJumpTime = Time.time;

                    // ВІДТВОРЕННЯ ЗВУКУ СТРИБКА З БАТУТА
                    if (audioSource != null && audioSource.clip != null)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                    }

                    Debug.Log("Гравець підкинутий батутом.");
                }
            }
        }
    }
}