using UnityEngine;

public class Shatterable : MonoBehaviour
{
    [Header("Настройки частиц")]
    public int fragmentCount = 20;
    public float fragmentSize = 0.15f;
    public string folderName = "=== MECHANICS ===";

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Shatter()
    {
        if (sr == null || sr.sprite == null) return;

        GameObject folder = GameObject.Find(folderName);

        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragObj = new GameObject("Fragment");
            fragObj.transform.position = transform.position;
            
            if (folder != null) fragObj.transform.SetParent(folder.transform);

            // Добавляем твой компонент Fragment (который уже есть в проекте)
            Fragment f = fragObj.AddComponent<Fragment>();
            Vector3 randomDir = Random.insideUnitCircle.normalized;
            
            // Используем спрайт и цвет текущего объекта
            f.Init(sr.sprite, sr.color, randomDir, Random.Range(4f, 8f), Random.Range(0.8f, 1.5f), fragmentSize, false);
        }

        // Выключаем сам объект после взрыва
        sr.enabled = false;
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        
        // Удаляем объект совсем через 2 секунды
        Destroy(gameObject, 2f);
    }
}