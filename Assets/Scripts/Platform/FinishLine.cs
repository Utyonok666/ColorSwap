using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Для работы с TextMeshPro, иначе удалить строку

// Handles the finish line sequence and victory routine
// Управляет последовательностью при пересечении финиша и показом экрана победы
public class FinishLine : MonoBehaviour
{
    [Header("Настройки финала")]
    public float liftSpeed = 2f;        // Скорость подъема вверх // Player lift speed
    public float shakeIntensity = 0.1f; // Сила тряски // Visual shake intensity
    public float waitBeforeShatter = 1.5f; // Время подъема перед эффектом // Time before shatter
    public GameObject victoryUI;        // Canvas с текстом "Thanks For Playing" // Victory UI canvas

    private bool _isFinishing = false;  // Флаг, чтобы событие сработало один раз // Ensure finish triggers only once

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что это игрок и финиш ещё не активирован
        // Check if collider is player and finish not yet triggered
        if (!_isFinishing && (other.CompareTag("Player") || other.tag.StartsWith("Player")))
        {
            _isFinishing = true;
            StartCoroutine(EpicFinishRoutine(other.gameObject));
        }
    }

    private IEnumerator EpicFinishRoutine(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Move moveScript = player.GetComponent<Move>();

        if (rb != null && moveScript != null)
        {
            // 1. Подготовка: отключаем управление и физику
            // Prepare: disable input and physics
            moveScript.SetInputState(false);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;

            // 2. Подъем и тряска визуальной части
            // Lift and shake visual part
            float timer = 0;
            Vector3 originalVisualPos = moveScript.visualPart.localPosition;

            while (timer < waitBeforeShatter)
            {
                // Поднимаем игрока вверх // Move player up
                player.transform.position += Vector3.up * liftSpeed * Time.deltaTime;

                // Тряска визуальной части // Shake visual part
                float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
                float shakeY = Random.Range(-shakeIntensity, shakeIntensity);
                moveScript.visualPart.localPosition = originalVisualPos + new Vector3(shakeX, shakeY, 0);

                timer += Time.deltaTime;
                yield return null;
            }

            // Возвращаем визуальную часть на место // Reset visual position
            moveScript.visualPart.localPosition = originalVisualPos;

            // 3. Рассыпание (эффект смерти) // Shatter death effect
            moveScript.PlayShatterDeath();

            // 4. Показываем текст победы // Show victory UI
            if (victoryUI != null) victoryUI.SetActive(true);

            // 5. Ждем и перезагружаем сцену // Wait and reload scene
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(0);
        }
    }
}