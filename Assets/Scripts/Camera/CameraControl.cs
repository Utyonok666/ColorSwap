using UnityEngine;

// Smooth camera follow system (Плавное следование камеры за игроком)
public class CameraControl : MonoBehaviour
{
    public Transform target; // Target to follow (Цель)
    public Vector3 offset = new Vector3(0, 2, -10); // Camera offset (Смещение камеры)
    public float smoothSpeed = 0.125f; // Follow smoothness (Плавность движения)

    void LateUpdate()
    {
        if (target != null && this.enabled)
        {
            // Calculate desired position relative to target (Рассчитать нужную позицию)
            Vector3 desiredPosition = target.position + offset;

            // Smoothly move camera to target (Плавно двигать камеру к цели)
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }

    // Instantly snap camera to target position (Мгновенно переместить камеру к цели)
    public void TeleportToTarget()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}