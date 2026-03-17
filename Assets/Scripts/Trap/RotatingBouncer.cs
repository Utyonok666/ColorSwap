using UnityEngine;
using System.Collections; 

// A rotating obstacle that bounces the player away upon collision
// Вращающееся препятствие, которое отталкивает игрока при столкновении
public class RotatingBouncer : MonoBehaviour
{
    [Header("Rotation Settings / Настройки вращения")]
    public float rotationSpeed = 60f; 

    [Header("Bounce Settings / Настройки отскока")]
    public float bounceForceX = 10f; 
    public float bounceForceY = 8f; 
    public float controlDisableTime = 0.3f; // Time player loses control after hit / Время потери управления после удара
    
    private Rigidbody2D _rb;
    private bool _isBouncing = false; 

    private void Awake()
    {
        // Setup Kinematic Rigidbody for consistent rotation physics
        // Настройка Kinematic Rigidbody для стабильной физики вращения
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null) {
            _rb.bodyType = RigidbodyType2D.Kinematic; 
            _rb.useFullKinematicContacts = true;
        }
    }

    private void FixedUpdate()
    {
        // Apply rotation via Rigidbody for better physics interaction
        // Применяем вращение через Rigidbody для лучшего взаимодействия с физикой
        _rb.MoveRotation(_rb.rotation + rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for any player variation (by name or color tags)
        // Проверка игрока (по имени или цветовым тегам)
        if (collision.gameObject.name.Contains("Player") || collision.gameObject.CompareTag("PlayerRed") || 
            collision.gameObject.CompareTag("PlayerYellow") || collision.gameObject.CompareTag("PlayerGreen"))
        {
            if (!_isBouncing)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                Move playerMove = collision.gameObject.GetComponent<Move>(); 
                
                if (playerRb != null && playerMove != null) 
                    StartCoroutine(ApplyBounce(playerRb, playerMove));
            }
        }
    }

    // Handles the knockback effect and temporary input lockout
    // Управляет эффектом отбрасывания и временной блокировкой ввода
    private IEnumerator ApplyBounce(Rigidbody2D playerRb, Move playerMove)
    {
        _isBouncing = true; 
        
        // Disable player control to prevent fighting the knockback
        // Отключаем управление, чтобы игрок не сопротивлялся отбрасыванию
        playerMove.SetInputState(false);

        // Determine bounce direction based on relative position
        // Определяем направление отскока относительно позиции объекта
        float side = playerRb.position.x < transform.position.x ? -1 : 1;
        
        playerRb.linearVelocity = Vector2.zero; // Reset current velocity for consistent bounce / Сброс скорости
        playerRb.AddForce(new Vector2(bounceForceX * side, bounceForceY), ForceMode2D.Impulse);

        yield return new WaitForSeconds(controlDisableTime);
        
        // Restore player control
        // Возвращаем управление игроку
        playerMove.SetInputState(true);
        _isBouncing = false; 
    }
}