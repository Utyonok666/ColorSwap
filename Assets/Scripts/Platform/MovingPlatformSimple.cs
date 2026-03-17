using UnityEngine;
using System.Collections;

// Moving platform that travels between two points, optionally only when the player is on it
// Двигающаяся платформа между двумя точками, может двигаться только когда игрок на ней
public class MovingPlatformSimple : MonoBehaviour
{
    private Vector2 _pointStart;  // Starting point of movement // Начальная точка движения

    [Header("Point Settings")]
    public Vector2 pointEnd;      // End point of movement // Конечная точка движения

    [Header("Movement Settings")]
    public float speed = 3f;      // Movement speed // Скорость движения
    public float delayTime = 1f;  // Pause time at points // Время задержки на точках

    [Header("Operation Mode")]
    public bool moveOnlyWhenPlayerOn = false; // Only move when player is on it // Двигаться только с игроком

    [Header("Animation Settings")]
    public float respawnScaleSpeed = 5f; // Speed of respawn animation // Скорость анимации восстановления

    private float _delayTimer;       // Timer for pauses // Таймер паузы
    private Vector2 _targetPosition; // Current target // Текущая цель
    private float _zFixPosition;     // Keep Z position constant // Фиксация Z позиции
    private bool _isPlayerOn = false; // Is player currently on platform // Игрок на платформе
    private bool _isMoving = false;  // Is platform currently moving // Движется ли платформа
    private bool _isResetting = false; // Is respawn animation running // Процесс восстановления
    private Vector3 _initialScale;   // Original scale for respawn animation // Начальный масштаб

    private void Start()
    {
        _pointStart = (Vector2)transform.position; // Record start position // Сохраняем стартовую позицию
        _zFixPosition = transform.position.z;      // Fix Z // Фиксируем Z
        _targetPosition = pointEnd;                // Initial target // Начальная цель
        _delayTimer = delayTime;                   // Set initial delay // Устанавливаем паузу
        _initialScale = transform.localScale;     // Store scale // Сохраняем масштаб
    }

    private void FixedUpdate()
    {
        if (_isResetting) return; // Skip movement during respawn // Не двигать во время восстановления

        if (moveOnlyWhenPlayerOn && !_isMoving && !_isPlayerOn && (Vector2)transform.position == _pointStart)
        {
            return; // Wait for player if required // Ждем игрока, если нужно
        }

        if (_isPlayerOn) _isMoving = true; // Start moving if player steps on // Двигаемся, если игрок на платформе

        if (_delayTimer > 0)
        {
            _delayTimer -= Time.fixedDeltaTime; // Countdown pause timer // Таймер паузы
            return;
        }

        // Move platform towards target // Двигаем платформу к цели
        Vector2 newPosition2D = Vector2.MoveTowards((Vector2)transform.position, _targetPosition, speed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPosition2D.x, newPosition2D.y, _zFixPosition);

        // Check if reached target // Проверяем достижение цели
        if (Vector2.Distance((Vector2)transform.position, _targetPosition) < 0.001f)
        {
            if (_targetPosition == pointEnd)
            {
                _targetPosition = _pointStart; // Switch target // Меняем цель
                _delayTimer = delayTime;        // Reset delay // Сбрасываем паузу
            }
            else
            {
                _targetPosition = pointEnd;
                _delayTimer = delayTime;
                _isMoving = false; // Stop movement if returned to start // Останавливаем движение
            }
        }
    }

    public void InstantRespawn()
    {
        if (_isResetting) return;
        StartCoroutine(RespawnRoutine()); // Start respawn animation // Запускаем анимацию восстановления
    }

    private IEnumerator RespawnRoutine()
    {
        _isResetting = true;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * respawnScaleSpeed * 2;
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t); // Shrink platform // Сжимаем платформу
            yield return null;
        }

        // Reset position and state // Сброс позиции и состояния
        transform.position = new Vector3(_pointStart.x, _pointStart.y, _zFixPosition);
        _targetPosition = pointEnd;
        _delayTimer = delayTime;
        _isMoving = false;

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * respawnScaleSpeed;
            transform.localScale = Vector3.Lerp(Vector3.zero, _initialScale, t); // Restore platform // Восстанавливаем платформу
            yield return null;
        }

        transform.localScale = _initialScale;
        _isResetting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Attach player to platform for smooth movement // Присоединяем игрока к платформе
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Player") || collision.gameObject.tag.StartsWith("Player"))
        {
            collision.transform.SetParent(transform);
            _isPlayerOn = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Detach player when leaving platform // Отсоединяем игрока
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Player") || collision.gameObject.tag.StartsWith("Player"))
        {
            if (collision.gameObject.activeInHierarchy) // Only detach if active // Только если объект активен
            {
                collision.transform.SetParent(null);
            }
            _isPlayerOn = false;
        }
    }
}