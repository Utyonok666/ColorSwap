using UnityEngine;

public class JellyZone : MonoBehaviour
{
    [Header("Настройки желе")]
    public float slowFactor = 0.5f; 
    public float exitBoost = 1.2f;  
    
    private AudioSource audioSource;

    private void Awake()
    {
        // Просто берем компонент, который ты повесил на объект в инспекторе
        audioSource = GetComponent<AudioSource>();
        
        // Проверяем, не забыл ли ты поставить галочку Loop, 
        // так как для зоны желе звук обычно должен зацикливаться
        if (audioSource != null)
        {
            audioSource.loop = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerYellow") || other.CompareTag("PlayerGreen") || other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Move move = other.GetComponent<Move>();
            if (rb != null && move != null)
            {
                move.SetMovementModifiers(slowFactor, 1f);
                rb.linearVelocity *= slowFactor;
                
                // Если звук еще не играет — включаем
                if (audioSource != null && !audioSource.isPlaying) 
                    audioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerYellow") || other.CompareTag("PlayerGreen") || other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Move move = other.GetComponent<Move>();
            if (rb != null && move != null)
            {
                move.SetMovementModifiers(1f, 1f);
                rb.linearVelocity *= exitBoost;
                
                // Игрок вышел — выключаем звук
                if (audioSource != null && audioSource.isPlaying) 
                    audioSource.Stop();
            }
        }
    }
}