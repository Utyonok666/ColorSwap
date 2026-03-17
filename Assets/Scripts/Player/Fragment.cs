using UnityEngine;

// Handles a single fragment for shatter effects (moving and fading/reforming)
// Управляет одним фрагментом для эффектов разрушения (движение, исчезновение/воссоздание)
public class Fragment : MonoBehaviour
{
    private SpriteRenderer sr;      // Sprite renderer for fragment // Спрайт рендер для фрагмента
    private Vector3 startOffset;    // Starting offset for reform animation // Начальное смещение при восстановлении
    private Vector3 targetDir;      // Direction to move if not reforming // Направление движения если не восстанавливается
    private float speed;            // Movement speed // Скорость движения
    private float fadeSpeed;        // Fade in/out speed // Скорость появления/исчезновения
    private bool isReforming;       // Whether fragment is reforming // Воссоздаётся ли фрагмент
    private float timer = 0;        // Timer for animations // Таймер для анимации

    // Initializes the fragment with given parameters
    // Инициализация фрагмента с заданными параметрами
    public void Init(Sprite sprite, Color color, Vector3 dir, float moveSpeed, float fade, float size, bool reform)
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = 10; // Ensure fragment renders above most objects // Рендерим поверх большинства объектов
        
        speed = moveSpeed;
        fadeSpeed = fade;
        isReforming = reform;
        transform.localScale = Vector3.one * size; // Scale fragment // Масштаб фрагмента

        if (isReforming) {
            // Setup for reform animation // Настройка для анимации восстановления
            startOffset = dir * speed * 0.4f;
            transform.localPosition = startOffset;
            sr.color = new Color(color.r, color.g, color.b, 0); // Start transparent // Начинаем прозрачным
        } else {
            // Setup for normal shatter movement // Настройка обычного разрушения
            targetDir = dir;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isReforming) {
            // Move fragment back to original position (reforming)
            // Двигаем фрагмент обратно к исходной позиции (воссоздание)
            transform.localPosition = Vector3.Lerp(startOffset, Vector3.zero, timer * speed * 0.8f);

            // Fade in the fragment // Появление фрагмента
            Color c = sr.color;
            c.a += fadeSpeed * Time.deltaTime;
            sr.color = c;

            // Destroy when done or close enough to target
            // Уничтожаем когда завершено или близко к цели
            if (timer > 1.2f || Vector3.Distance(transform.localPosition, Vector3.zero) < 0.05f)
                Destroy(gameObject);
        } else {
            // Move fragment in direction and rotate // Движение и вращение фрагмента
            transform.position += targetDir * speed * Time.deltaTime;
            transform.Rotate(0, 0, 200 * Time.deltaTime);

            // Fade out over time // Исчезновение со временем
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;

            // Destroy when fully transparent // Уничтожаем при полной прозрачности
            if (c.a <= 0)
                Destroy(gameObject);
        }
    }
}