using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.StartsWith("Player")) Respawn(other.gameObject);
    }

    void Respawn(GameObject player)
    {
        player.transform.position = Checkpoint.LastCheckPointPos;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (PlatformManager.Instance != null)
            PlatformManager.Instance.NotifyColorChange(player.tag);

        if (Camera.main != null)
        {
            var cc = Camera.main.GetComponent<CameraControl>();
            if (cc != null) cc.TeleportToTarget();
        }
    }
}