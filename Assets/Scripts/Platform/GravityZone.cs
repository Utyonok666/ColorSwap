using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GravityZone : MonoBehaviour
{
    public enum ZoneMode { Push, PortalStream }

    [Header("Режим работы")]
    public ZoneMode mode = ZoneMode.Push;

    [Header("Настройки направления")]
    public Vector2 direction = Vector2.up;

    [Header("Настройки скорости")]
    public float speedOrForce = 20f; 
    public float maxVelocity = 15f;  
    [Range(1f, 50f)] public float captureTightness = 20f; // Увеличил диапазон для "мертвой хватки"

    [Header("Физика")]
    public bool disableGravityInZone = true; 
    public float linearDamping = 5f;

    [Header("Фильтры")]
    public LayerMask affectedLayers; 
    public bool useTagsInstead = true; 

    private float _defaultDrag;
    private float _defaultGravity;

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
                rb.WakeUp(); // Не даем физике "уснуть"

                if (disableGravityInZone) rb.gravityScale = 0;

                if (mode == ZoneMode.Push)
                {
                    rb.AddForce(direction.normalized * speedOrForce);
                    if (rb.linearVelocity.magnitude > maxVelocity)
                        rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
                    
                    rb.linearDamping = linearDamping;
                }
                else // PortalStream (Исправлено сползание)
                {
                    Vector2 targetVel = direction.normalized * speedOrForce;
                    
                    // Жесткое перемещение по вектору
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, Time.fixedDeltaTime * captureTightness);

                    // КОРРЕКЦИЯ СПОЛЗАНИЯ: 
                    // Если поток идет строго по горизонтали, обнуляем Y принудительно
                    if (Mathf.Abs(direction.y) < 0.1f)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                    }
                    // Если поток идет строго по вертикали, обнуляем X
                    else if (Mathf.Abs(direction.x) < 0.1f)
                    {
                        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    }
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
                _defaultDrag = rb.linearDamping;
                _defaultGravity = rb.gravityScale;
                
                if (mode == ZoneMode.PortalStream) rb.linearVelocity = Vector2.zero;
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
                rb.linearDamping = _defaultDrag;
                rb.gravityScale = _defaultGravity;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (mode == ZoneMode.PortalStream) ? Color.blue : Color.yellow;
        Vector3 pos = transform.position;
        Gizmos.DrawRay(pos, (Vector3)direction.normalized * 2f);
        Gizmos.DrawWireSphere(pos + (Vector3)direction.normalized * 2f, 0.2f);
    }
}
