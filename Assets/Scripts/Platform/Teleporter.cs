using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    public Transform exitPoint; 
    public float teleportCooldown = 0.5f; 
    private static bool _isTeleporting = false;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTeleporting) return;

        // Оставляем проверку тегов, как было
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerYellow") || 
            other.CompareTag("PlayerGreen") || other.CompareTag("Player"))
        {
            if (exitPoint != null)
            {
                StartCoroutine(DoTeleport(other.transform));
            }
        }
    }

    private IEnumerator DoTeleport(Transform player)
    {
        _isTeleporting = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // Переносим игрока, фиксируя Z на 0 (стандарт для 2D)
        player.position = new Vector3(exitPoint.position.x, exitPoint.position.y, 0f);

        if (Camera.main != null)
        {
            var cam = Camera.main.GetComponent<CameraControl>();
            if (cam != null) cam.TeleportToTarget();
        }

        yield return new WaitForSeconds(teleportCooldown);
        _isTeleporting = false;
    }
}