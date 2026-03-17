using UnityEngine;
using System.Collections;

// Manages laser obstacles with warning phases, timed cycles, and damage logic
// Управляет лазерными препятствиями: фазы предупреждения, циклы работы и логика урона
[RequireComponent(typeof(LineRenderer))]
public class LaserDestroyer : MonoBehaviour
{
    [Header("Damage Settings / Настройки урона")]
    public bool canKillPlayer = true;       // Toggle: can kill the player / Галочка: убивать игрока
    public bool canDestroyObjects = true;   // Toggle: can break shatterable objects / Галочка: ломать ящики (Shatterable)

    [Header("Sync Settings / Настройки синхронизации")]
    public float startDelay = 0f;           // Initial delay before starting the cycle / Задержка старта для очередности

    [Header("Timer Settings (Seconds) / Настройки времени")]
    public bool useTimer = true;
    public float timeInactive = 2.0f;       // Laser is off / Сколько лазер спит
    public float timeWarning = 1.5f;        // Warning beam is visible / Сколько висит предупреждение (ромб)
    public float timeActive = 2.0f;         // Laser is deadly / Сколько лазер убивает

    [Header("Beam Appearance / Настройки луча")]
    public Color laserColor = Color.red;
    public Color warningColor = new Color(1, 0, 0, 0.2f); 
    public float laserWidth = 0.1f;
    public float warningWidth = 0.02f; 

    [Header("Targets & Spawn / Цели и спавн")]
    public GameObject prefabToSpawn; 
    public Transform spawnPoint;
    public MovingPlatformSimple targetPlatform;
    public LayerMask hitLayers;
    public float maxDistance = 50f;

    private LineRenderer _lineRenderer;
    private bool _isProcessingDeath = false;
    private bool _isActive = false;      
    private bool _isWarning = false;     

    private void Awake()
    {
        // Initialize LineRenderer component and its material
        // Инициализируем LineRenderer и его материал
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.sortingOrder = 5; 
    }

    private void Start()
    {
        // Start the timed cycle if enabled
        // Запускаем цикл таймера, если он включен
        if (useTimer) StartCoroutine(DelayedStart());
    }

    // Handles the initial offset delay
    // Обрабатывает начальную задержку смещения
    private IEnumerator DelayedStart()
    {
        if (startDelay > 0)
        {
            _lineRenderer.enabled = false;
            yield return new WaitForSeconds(startDelay);
        }
        
        StartCoroutine(LaserCycle());
    }

    // Main laser state machine: Inactive -> Warning -> Active
    // Главный цикл состояний лазера: Выключен -> Предупреждение -> Активен
    private IEnumerator LaserCycle()
    {
        while (true)
        {
            // 1. INACTIVE state
            // 1. ВЫКЛЮЧЕН
            _isActive = false;
            _isWarning = false;
            _lineRenderer.enabled = false;
            yield return new WaitForSeconds(timeInactive);

            // 2. WARNING state (visual only, safe)
            // 2. ПРЕДУПРЕЖДЕНИЕ (только визуал, безопасно)
            _isWarning = true;
            _lineRenderer.enabled = true;
            SetBeamSettings(warningColor, warningWidth);
            yield return new WaitForSeconds(timeWarning);

            // 3. ACTIVE state (deadly)
            // 3. АКТИВЕН (убивает)
            _isWarning = false;
            _isActive = true;
            SetBeamSettings(laserColor, laserWidth);
            yield return new WaitForSeconds(timeActive);
        }
    }

    // Updates visual properties of the laser beam
    // Обновляет визуальные свойства луча
    private void SetBeamSettings(Color color, float width)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
    }

    private void Update()
    {
        // Calculate laser path if it's visible
        // Рассчитываем путь лазера, если он виден
        if (_isActive || _isWarning) ShootLaser();
    }

    // Performs raycasting and handles hit detection logic
    // Выполняет рейкаст и обрабатывает логику попадания
    private void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * 0.1f, transform.right, maxDistance, hitLayers);
        _lineRenderer.SetPosition(0, transform.position);

        if (hit.collider != null)
        {
            _lineRenderer.SetPosition(1, hit.point);

            // Skip damage logic if in warning phase
            // Пропускаем логику урона в фазе предупреждения
            if (_isWarning) return; 

            if (!_isProcessingDeath)
            {
                GameObject obj = hit.collider.gameObject;

                // Check for interactive platforms
                // Проверка интерактивных платформ
                if (targetPlatform != null && obj == targetPlatform.gameObject)
                {
                    targetPlatform.InstantRespawn();
                    return;
                }

                // Identify if target is the player (various color tags)
                // Определяем, является ли цель игроком (разные цветовые теги)
                bool isPlayer = obj.CompareTag("Player") || obj.CompareTag("PlayerRed") || 
                                obj.CompareTag("PlayerYellow") || obj.CompareTag("PlayerGreen");
                
                Shatterable shatterComp = obj.GetComponent<Shatterable>();

                // Check permissions to damage specific object types
                // Проверяем разрешения на нанесение урона конкретным типам объектов
                bool shouldKillPlayer = isPlayer && canKillPlayer;
                bool shouldDestroyObject = shatterComp != null && canDestroyObjects;

                if (shouldKillPlayer || shouldDestroyObject)
                {
                    StartCoroutine(ProcessObjectDeath(obj, isPlayer, shatterComp));
                }
            }
        }
        else
        {
            // If nothing hit, draw beam to max distance
            // Если попаданий нет, рисуем луч на максимальную дистанцию
            _lineRenderer.SetPosition(1, transform.position + transform.right * maxDistance);
        }
    }

    // Manages the destruction and respawn sequence for players and objects
    // Управляет последовательностью разрушения и возрождения игроков и объектов
    private IEnumerator ProcessObjectDeath(GameObject target, bool isPlayer, Shatterable sh)
    {
        _isProcessingDeath = true;
        
        // Trigger destruction effects
        // Запускаем эффекты разрушения
        if (sh != null) sh.Shatter();
        else
        {
            Move mv = target.GetComponent<Move>();
            if (mv != null) mv.PlayShatterDeath();
        }

        // Only handle respawn logic if the target is a player
        // Логика возрождения только для игрока
        if (!isPlayer)
        {
            _isProcessingDeath = false;
            yield break;
        }

        // Wait for death animation, then teleport to checkpoint
        // Ждем анимацию смерти, затем телепортируем на чекпоинт
        yield return new WaitForSeconds(0.8f);
        Vector3 spawnPos = Checkpoint.LastCheckPointPos;
        
        if (spawnPos != Vector3.zero)
        {
            target.transform.position = spawnPos;
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            if (rb != null) { rb.simulated = true; rb.linearVelocity = Vector2.zero; }
            
            yield return new WaitForSeconds(0.5f);
            
            // Play reform animation and reset state
            // Проигрываем анимацию восстановления и сбрасываем состояние
            Move mv = target.GetComponent<Move>();
            if (mv != null) 
            { 
                mv.PlayReformAnimation(); 
                yield return new WaitForSeconds(0.6f); 
                mv.ResetVisuals(); 
            }
        }
        _isProcessingDeath = false;
    }
}