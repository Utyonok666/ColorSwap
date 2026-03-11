using UnityEngine;
using System.Collections;

public class MovingPlatformSimple : MonoBehaviour
{
    private Vector2 _pointStart; 
    
    [Header("Настройки точек")]
    public Vector2 pointEnd;   

    [Header("Настройки движения")]
    public float speed = 3f; 
    public float delayTime = 1f;  

    [Header("Режим работы")]
    public bool moveOnlyWhenPlayerOn = false;

    [Header("Настройки анимации")]
    public float respawnScaleSpeed = 5f;

    private float _delayTimer; 
    private Vector2 _targetPosition; 
    private float _zFixPosition; 
    private bool _isPlayerOn = false; 
    private bool _isMoving = false; 
    private bool _isResetting = false; // Флаг процесса уничтожения
    private Vector3 _initialScale;

    private void Start()
    {
        _pointStart = (Vector2)transform.position; 
        _zFixPosition = transform.position.z;
        _targetPosition = pointEnd;
        _delayTimer = delayTime; 
        _initialScale = transform.localScale; 
    }

    private void FixedUpdate()
    {
        if (_isResetting) return;

        if (moveOnlyWhenPlayerOn && !_isMoving && !_isPlayerOn && (Vector2)transform.position == _pointStart)
        {
            return;
        }

        if (_isPlayerOn) _isMoving = true;

        if (_delayTimer > 0)
        {
            _delayTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 newPosition2D = Vector2.MoveTowards((Vector2)transform.position, _targetPosition, speed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPosition2D.x, newPosition2D.y, _zFixPosition);

        if (Vector2.Distance((Vector2)transform.position, _targetPosition) < 0.001f)
        {
            if (_targetPosition == pointEnd)
            {
                _targetPosition = _pointStart;
                _delayTimer = delayTime;
            }
            else
            {
                _targetPosition = pointEnd;
                _delayTimer = delayTime;
                _isMoving = false; 
            }
        }
    }

    public void InstantRespawn()
    {
        if (_isResetting) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        _isResetting = true;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * respawnScaleSpeed * 2;
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t);
            yield return null;
        }

        transform.position = new Vector3(_pointStart.x, _pointStart.y, _zFixPosition);
        _targetPosition = pointEnd;
        _delayTimer = delayTime;
        _isMoving = false;

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * respawnScaleSpeed;
            transform.localScale = Vector3.Lerp(Vector3.zero, _initialScale, t);
            yield return null;
        }

        transform.localScale = _initialScale;
        _isResetting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверка по тегу или имени (для надежности)
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Player") || collision.gameObject.tag.StartsWith("Player"))
        {
            collision.transform.SetParent(transform);
            _isPlayerOn = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Player") || collision.gameObject.tag.StartsWith("Player"))
        {
            // --- ИСПРАВЛЕНИЕ: ПРОВЕРКА АКТИВНОСТИ ---
            // Если игрок умер от лазера, он деактивируется. Unity запрещает менять родителя у неактивных объектов.
            if (collision.gameObject.activeInHierarchy)
            {
                collision.transform.SetParent(null);
            }
            // ----------------------------------------
            
            _isPlayerOn = false;
        }
    }
}