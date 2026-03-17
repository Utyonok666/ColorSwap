using UnityEngine;

// Handles breaking objects into fragments / Скрипт для разрушения объекта на фрагменты
public class Shatterable : MonoBehaviour
{
    [Header("Particle Settings / Настройки частиц")]
    public int fragmentCount = 20;          // Number of fragments / Количество фрагментов
    public float fragmentSize = 0.15f;      // Size of each fragment / Размер фрагментов
    public string folderName = "=== MECHANICS ==="; // Parent folder for fragments / Папка для фрагментов

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Shatter()
    {
        // Safety check / Проверка на наличие спрайта
        if (sr == null || sr.sprite == null) return;

        // Find or assign parent folder for fragments / Находим папку для фрагментов
        GameObject folder = GameObject.Find(folderName);

        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragObj = new GameObject("Fragment");
            fragObj.transform.position = transform.position;

            if (folder != null) fragObj.transform.SetParent(folder.transform);

            // Add your Fragment component / Добавляем компонент Fragment
            Fragment f = fragObj.AddComponent<Fragment>();
            Vector3 randomDir = Random.insideUnitCircle.normalized;

            // Initialize fragment with sprite, color, direction, speed, fade, size, not reforming / Настраиваем фрагмент
            f.Init(sr.sprite, sr.color, randomDir, Random.Range(4f, 8f), Random.Range(0.8f, 1.5f), fragmentSize, false);
        }

        // Disable the main object after shattering / Выключаем спрайт и коллайдер объекта
        sr.enabled = false;
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        // Destroy the object completely after 2 seconds / Удаляем объект через 2 секунды
        Destroy(gameObject, 2f);
    }
}