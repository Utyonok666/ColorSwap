using UnityEngine;
using System.Collections;

// Player movement controller with physics, jumping, squash/stretch, boost, and shatter effects
// Контроллер движения игрока с физикой, прыжками, эффектом сжатия/растяжения, бустом и разрушением
public class Move : MonoBehaviour
{
    [Header("1. Movement Physics / Физика движения")]
    public float maxSpeed = 8f;         // Maximum horizontal speed // Максимальная горизонтальная скорость
    public float accelForce = 40f;      // Acceleration force applied when moving // Сила ускорения при движении
    public float friction = 15f;        // Deceleration when no input // Торможение без ввода
    public float jumpForce = 12f;       // Jump impulse // Сила прыжка

    [Header("Mario-style Jump / Прыжки в стиле Марио")]
    public float fallMultiplier = 1.5f;     // Faster fall gravity multiplier // Ускорение падения
    public float lowJumpMultiplier = 8f;    // Higher jump gravity when tap is short // Высота прыжка при коротком тапе

    [Header("2. Visual Squash & Tilt / Визуальные эффекты")]
    public Transform visualPart;            // Part to squash/tilt // Часть, которая сжимается и наклоняется
    public float squashSpeed = 15f;         // Speed of squash animation // Скорость анимации
    public float tiltAngle = 15f;           // Max tilt on movement // Максимальный наклон
    public Vector3 jumpScale = new Vector3(0.7f, 1.3f, 1f); // Scale when jumping // Масштаб при прыжке
    public Vector3 landScale = new Vector3(1.3f, 0.7f, 1f); // Scale when landing // Масштаб при приземлении

    [Header("3. Particle Effects / Частицы")]
    public int fragmentCount = 25;          // Number of fragments when shattering // Кол-во фрагментов
    public float fragmentSize = 0.15f;      // Size of each fragment // Размер фрагмента

    private Rigidbody2D rb;                 // Rigidbody for physics // Rigidbody для физики
    private SpriteRenderer myRenderer;      // Main sprite renderer // Основной спрайт
    private SpriteRenderer childRenderer;   // Visual child renderer // Рендер визуальной части

    private float moveInputX;
    private float moveInputY;
    private bool isGrounded;
    private bool _canControl = true;
    private float _speedModifier = 1f;       // Modifier for slow/boost effects // Модификатор скорости
    private float _boostTimer = 0f;          // Timer for temporary boost // Таймер буста
    private AudioSource audioSource;          // Audio source for jump/feedback // Источник звука

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        myRenderer = GetComponent<SpriteRenderer>();

        if (visualPart == null && transform.childCount > 0)
            visualPart = transform.GetChild(0);

        if (visualPart != null)
            childRenderer = visualPart.GetComponent<SpriteRenderer>();
    }

    // --- Compatibility methods for external calls ---
    // --- Методы для совместимости с другими системами ---
    public void PlayBurstAnimation(float duration) => PlayShatterDeath();
    public void PlayDeathAnimation() => PlayShatterDeath();

    public void PlayReformAnimation()
    {
        if (childRenderer == null) return;

        childRenderer.enabled = false;
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragObj = new GameObject("ReformFragment");
            fragObj.transform.SetParent(this.transform);
            Fragment f = fragObj.AddComponent<Fragment>();
            Vector3 randomDir = Random.insideUnitCircle.normalized;
            f.Init(childRenderer.sprite, childRenderer.color, randomDir, Random.Range(3f, 5f), 2f, fragmentSize, true);
        }
    }

    // --- Boost / Буст ---
    public void ApplyBoost(float duration) => _boostTimer = duration;

    public void ResetBoost()
    {
        StopAllCoroutines();
        StartCoroutine(FullPhysicsResetRoutine());
    }

    private IEnumerator FullPhysicsResetRoutine()
    {
        _boostTimer = 0f;
        _canControl = false;

        for (int i = 0; i < 2; i++)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.totalForce = Vector2.zero; // Reset all forces // Сброс всех сил
                rb.Sleep();
            }
            yield return new WaitForFixedUpdate();
        }

        _canControl = true;
    }

    public void SetMovementModifiers(float speedMod, float jumpMod) => _speedModifier = speedMod;

    // --- Shatter Death / Разрушение ---
    public void PlayShatterDeath()
    {
        if (childRenderer == null) return;

        GameObject playerFolder = GameObject.Find("=== PLAYER ===");

        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragObj = new GameObject("DeathFragment");
            fragObj.transform.position = transform.position;
            if (playerFolder != null) fragObj.transform.SetParent(playerFolder.transform);

            Fragment f = fragObj.AddComponent<Fragment>();
            Vector3 randomDir = Random.insideUnitCircle.normalized;
            f.Init(childRenderer.sprite, childRenderer.color, randomDir, Random.Range(4f, 8f), Random.Range(0.8f, 1.5f), fragmentSize, false);
        }

        childRenderer.enabled = false;
        SetInputState(false);
        rb.simulated = false;
    }

    public void ResetVisuals()
    {
        childRenderer.enabled = true;
        SetInputState(true);
        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        transform.localScale = Vector3.one;
        if (visualPart != null) visualPart.localScale = Vector3.one;
        ResetBoost();
    }

    // --- Input and Visual Feedback / Ввод и визуальные эффекты ---
    void Update()
    {
        if (visualPart != null && _canControl)
        {
            // Smooth squash back to normal scale // Плавное возвращение масштаба
            visualPart.localScale = Vector3.Lerp(visualPart.localScale, Vector3.one, Time.deltaTime * squashSpeed);

            // Tilt based on movement // Наклон в зависимости от движения
            float targetRotation = -moveInputX * tiltAngle;
            visualPart.localRotation = Quaternion.Lerp(visualPart.localRotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * (squashSpeed / 2));

            // Sync child sprite with main renderer // Синхронизация спрайта с основным рендером
            if (myRenderer != null && childRenderer != null)
            {
                childRenderer.color = myRenderer.color;
                childRenderer.sprite = myRenderer.sprite;
                if (myRenderer.enabled) myRenderer.enabled = false;
            }
        }

        if (!_canControl) return;

        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) Jump();
    }

    // --- Physics movement / Физическое движение ---
    void FixedUpdate()
    {
        if (!_canControl) { rb.linearVelocity = Vector2.zero; return; }

        // Variable jump height // Разная высота прыжка
        if (rb.linearVelocity.y > 0.1f && !Input.GetKey(KeyCode.Space))
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y < -0.1f)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;

        if (_boostTimer > 0) _boostTimer -= Time.fixedDeltaTime;

        float currentMaxSpeed = maxSpeed * _speedModifier;
        if (Input.GetKey(KeyCode.LeftShift)) currentMaxSpeed *= 1.5f;
        if (_boostTimer > 0) currentMaxSpeed *= 3f;

        float targetSpeedX = moveInputX * currentMaxSpeed;
        float speedDifX = targetSpeedX - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(moveInputX) > 0.01f) ? accelForce : friction;
        rb.AddForce(speedDifX * accelRate * Vector2.right, ForceMode2D.Force);

        // Vertical movement for gravity-less zones // Вертикальное движение для зон без гравитации
        if (Mathf.Abs(rb.gravityScale) < 0.1f)
        {
            float targetSpeedY = moveInputY * currentMaxSpeed;
            float speedDifY = targetSpeedY - rb.linearVelocity.y;
            rb.AddForce(speedDifY * accelForce * Vector2.up, ForceMode2D.Force);
        }
    }

    // --- Ground check / Проверка на земле ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                if (visualPart != null) visualPart.localScale = landScale;
                return;
            }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
            if (contact.normal.y > 0.5f) { isGrounded = true; return; }
    }

    private void OnCollisionExit2D(Collision2D collision) => isGrounded = false;

    // --- Jump / Прыжок ---
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        if (visualPart != null) visualPart.localScale = jumpScale;
        if (audioSource != null && audioSource.clip != null) audioSource.PlayOneShot(audioSource.clip);
    }

    // --- Enable/Disable player input / Включение/отключение управления ---
    public void SetInputState(bool state)
    {
        _canControl = state;
        if (!state)
        {
            moveInputX = 0;
            moveInputY = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }
}