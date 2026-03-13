using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Если используешь TextMeshPro, иначе удали эту строку

public class FinishLine : MonoBehaviour
{
    [Header("Настройки финала")]
    public float liftSpeed = 2f;        // Скорость подъема вверх
    public float shakeIntensity = 0.1f; // Сила тряски
    public float waitBeforeShatter = 1.5f; // Сколько секунд лететь перед взрывом
    public GameObject victoryUI;        // Ссылка на Canvas с текстом "Thanks For Playing"

    private bool _isFinishing = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
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
            // 1. Подготовка: отключаем управление и физику (чтобы не падал)
            moveScript.SetInputState(false);
            rb.bodyType = RigidbodyType2D.Kinematic; // Делаем его "невесомым"
            rb.linearVelocity = Vector2.zero;

            // 2. Фаза подъема и тряски
            float timer = 0;
            Vector3 originalVisualPos = moveScript.visualPart.localPosition;

            while (timer < waitBeforeShatter)
            {
                // Поднимаем игрока вверх
                player.transform.position += Vector3.up * liftSpeed * Time.deltaTime;

                // Трясем визуальную часть (visualPart из Move.cs)
                float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
                float shakeY = Random.Range(-shakeIntensity, shakeIntensity);
                moveScript.visualPart.localPosition = originalVisualPos + new Vector3(shakeX, shakeY, 0);

                timer += Time.deltaTime;
                yield return null;
            }

            // Возвращаем визуальную часть на место перед взрывом
            moveScript.visualPart.localPosition = originalVisualPos;

            // 3. Рассыпание
            moveScript.PlayShatterDeath();

            // 4. Показываем текст "Thanks For Playing"
            if (victoryUI != null) victoryUI.SetActive(true);

            // 5. Ждем немного и перезагружаем (или выходим)
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(0); 
        }
    }
}