using UnityEngine;

[RequireComponent(typeof(Collider2D))]
// Handles gravity zones and directional streams affecting the player
// Управляет зонами гравитации и потоками, влияющими на игрока
public class GravityZone : MonoBehaviour
{
    public enum ZoneMode { Push, PortalStream } // Mode: push or portal stream // Режим: толчок или поток 

    [Header("Режим работы")]
    public ZoneMode mode = ZoneMode.Push; // Active zone mode // Выбранный режим зоны 

    [Header("Настройки направления")]
    public Vector2 direction = Vector2.up; // Direction of force/stream // Направление силы или потока 

    [Header("Настройки скорости")]
    public float speedOrForce = 20f; // Force or stream speed // Сила толчка или скорость потока 
    public float maxVelocity = 15f;  // Max speed in Push mode // Максимальная скорость для Push 
    [Range(1f, 50f)] public float captureTightness = 20f; // Lerp tightness // Жесткость "схватывания" в PortalStream 

    [Header("Физика")]
    public bool disableGravityInZone = true; // Disable gravity inside zone // Отключаем гравитацию игрока в зоне 
    public float linearDamping = 5f; // Linear damping in zone // Линейное сопротивление в зоне 

    [Header("Фильтры")]
    public LayerMask affectedLayers; // Layers affected // Слои объектов, на которые влияет зона 
    public bool useTagsInstead = true; // Use tags instead of layers // Использовать теги вместо слоев 

    private float _defaultDrag;    // Original drag // Сохраняем исходное сопротивление 
    private float _defaultGravity; // Original gravity // Сохраняем исходную гравитацию 

    // Checks if the object is a player
    // Проверка, является ли объект игроком
    private bool IsPlayer(GameObject obj)
    {
        if (useTagsInstead) return obj.tag.StartsWith("Player");
        return ((1 << obj.layer) & affectedLayers) != 0;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsPlayer(other.gameObject))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.WakeUp(); // Prevent physics sleep // Не даем физике "уснуть" 

                if (disableGravityInZone) rb.gravityScale = 0; // Disable gravity // Отключаем гравитацию 

                if (mode == ZoneMode.Push)
                {
                    // Push player // Толкаем игрока в заданном направлении 
                    rb.AddForce(direction.normalized * speedOrForce);

                    // Limit max velocity // Ограничение максимальной скорости 
                    if (rb.linearVelocity.magnitude > maxVelocity)
                        rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;

                    rb.linearDamping = linearDamping;
                }
                else // PortalStream
                {
                    // Target velocity along stream // Целевая скорость по направлению потока 
                    Vector2 targetVel = direction.normalized * speedOrForce;

                    // Smoothly move to target velocity // Плавное "схватывание" скорости 
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, Time.fixedDeltaTime * captureTightness);

                    // Коррекция сползания: если строго горизонтально/вертикально, обнуляем лишнюю ось
                    if (Mathf.Abs(direction.y) < 0.1f) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                    else if (Mathf.Abs(direction.x) < 0.1f) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other.gameObject))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Save original physics values // Сохраняем исходные значения физики 
                _defaultDrag = rb.linearDamping;
                _defaultGravity = rb.gravityScale;

                if (mode == ZoneMode.PortalStream) rb.linearVelocity = Vector2.zero; // Reset velocity // Сброс скорости для потока 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other.gameObject))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Restore physics values // Восстанавливаем физику при выходе из зоны 
                rb.linearDamping = _defaultDrag;
                rb.gravityScale = _defaultGravity;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Zone color: blue=PortalStream, yellow=Push // Цвет зоны: синий для PortalStream, желтый для Push 
        Gizmos.color = (mode == ZoneMode.PortalStream) ? Color.blue : Color.yellow;
        Vector3 pos = transform.position;

        // Direction ray // Вектор направления 
        Gizmos.DrawRay(pos, (Vector3)direction.normalized * 2f);
        // Sphere at end of ray // Сфера на конце направления 
        Gizmos.DrawWireSphere(pos + (Vector3)direction.normalized * 2f, 0.2f);
    }
}