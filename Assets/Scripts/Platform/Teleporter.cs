using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    public Transform exitPoint; 
    public float teleportCooldown = 0.5f; 
    private static bool _isTeleporting = false;

    // Додаємо змінну для звуку
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTeleporting) return;

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

        // ВІДТВОРЕННЯ ЗВУКУ ТЕЛЕПОРТАЦІЇ
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        player.position = exitPoint.position;

        if (Camera.main != null)
        {
            var cam = Camera.main.GetComponent<CameraControl>();
            if (cam != null) cam.TeleportToTarget();
        }

        yield return new WaitForSeconds(teleportCooldown);
        _isTeleporting = false;
    }
}