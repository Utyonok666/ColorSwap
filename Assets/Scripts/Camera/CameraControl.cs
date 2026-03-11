using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target; // Цель следования (игрок)
    public Vector3 offset = new Vector3(0, 2, -10);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target != null && this.enabled)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }

    public void TeleportToTarget()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}