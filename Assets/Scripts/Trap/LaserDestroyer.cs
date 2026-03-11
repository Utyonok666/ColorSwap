using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LaserDestroyer : MonoBehaviour
{
    [Header("Настройки урона")]
    public bool canKillPlayer = true;      // Галочка: убивать игрока
    public bool canDestroyObjects = true;   // Галочка: ломать ящики (Shatterable)

    [Header("Настройки синхронизации")]
    public float startDelay = 0f;          // ЗАДЕРЖКА СТАРТА (для очередности)

    [Header("Настройки времени (в секундах)")]
    public bool useTimer = true;
    public float timeInactive = 2.0f;      // Сколько лазер спит
    public float timeWarning = 1.5f;       // Сколько висит предупреждение (ромб)
    public float timeActive = 2.0f;        // Сколько лазер убивает

    [Header("Настройки луча")]
    public Color laserColor = Color.red;
    public Color warningColor = new Color(1, 0, 0, 0.2f); 
    public float laserWidth = 0.1f;
    public float warningWidth = 0.02f; 

    [Header("Цели и спавн")]
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
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.sortingOrder = 5; 
    }

    private void Start()
    {
        if (useTimer) StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Сначала ждем стартовую задержку
        if (startDelay > 0)
        {
            _lineRenderer.enabled = false;
            yield return new WaitForSeconds(startDelay);
        }
        
        // Только потом запускаем бесконечный цикл
        StartCoroutine(LaserCycle());
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            // 1. ВЫКЛЮЧЕН
            _isActive = false;
            _isWarning = false;
            _lineRenderer.enabled = false;
            yield return new WaitForSeconds(timeInactive);

            // 2. ПРЕДУПРЕЖДЕНИЕ (безопасно)
            _isWarning = true;
            _lineRenderer.enabled = true;
            SetBeamSettings(warningColor, warningWidth);
            yield return new WaitForSeconds(timeWarning);

            // 3. АКТИВЕН (убивает, если галочка стоит)
            _isWarning = false;
            _isActive = true;
            SetBeamSettings(laserColor, laserWidth);
            yield return new WaitForSeconds(timeActive);
        }
    }

    private void SetBeamSettings(Color color, float width)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
    }

    private void Update()
    {
        if (_isActive || _isWarning) ShootLaser();
    }

    private void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * 0.1f, transform.right, maxDistance, hitLayers);
        _lineRenderer.SetPosition(0, transform.position);

        if (hit.collider != null)
        {
            _lineRenderer.SetPosition(1, hit.point);

            if (_isWarning) return; // В фазе предупреждения логика убийства не работает

            if (!_isProcessingDeath)
            {
                GameObject obj = hit.collider.gameObject;

                // Проверка платформы
                if (targetPlatform != null && obj == targetPlatform.gameObject)
                {
                    targetPlatform.InstantRespawn();
                    return;
                }

                // Проверка игрока
                bool isPlayer = obj.CompareTag("Player") || obj.CompareTag("PlayerRed") || 
                                obj.CompareTag("PlayerYellow") || obj.CompareTag("PlayerGreen");
                
                // Проверка разрушаемого объекта
                Shatterable shatterComp = obj.GetComponent<Shatterable>();

                // Убиваем/ломаем только если разрешено галочками
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
            _lineRenderer.SetPosition(1, transform.position + transform.right * maxDistance);
        }
    }

    private IEnumerator ProcessObjectDeath(GameObject target, bool isPlayer, Shatterable sh)
    {
        _isProcessingDeath = true;
        
        if (sh != null) sh.Shatter();
        else
        {
            Move mv = target.GetComponent<Move>();
            if (mv != null) mv.PlayShatterDeath();
        }

        if (!isPlayer)
        {
            _isProcessingDeath = false;
            yield break;
        }

        yield return new WaitForSeconds(0.8f);
        Vector3 spawnPos = Checkpoint.LastCheckPointPos;
        
        if (spawnPos != Vector3.zero)
        {
            target.transform.position = spawnPos;
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            if (rb != null) { rb.simulated = true; rb.linearVelocity = Vector2.zero; }
            yield return new WaitForSeconds(0.5f);
            Move mv = target.GetComponent<Move>();
            if (mv != null) { mv.PlayReformAnimation(); yield return new WaitForSeconds(0.6f); mv.ResetVisuals(); }
        }
        _isProcessingDeath = false;
    }
}