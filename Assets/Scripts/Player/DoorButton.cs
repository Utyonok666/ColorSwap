using UnityEngine;
using System.Collections;

public class DoorButton : MonoBehaviour
{
    [Header("Объекты")]
    public GameObject door;           // Стенка/дверь
    public GameObject hintPrompt;     // Подсказка "Нажми E"
    public Shatterable objectToShatter; // Объект для разрушения (опционально)

    [Header("Настройки движения")]
    public Vector3 slideOffset = new Vector3(0, -4, 0); 
    public float speed = 3f;                            

    private bool isPlayerInside = false;
    private bool isActivated = false; // Блокировка на время движения
    private bool isOpen = false;      // Состояние двери (открыта или закрыта)
    
    private Vector3 startPosition;    // Начальная точка (закрыто)
    private Vector3 endPosition;      // Конечная точка (открыто)

    void Start()
    {
        if (door != null) 
        {
            startPosition = door.transform.position;
            endPosition = startPosition + slideOffset;
        }
        
        if (hintPrompt != null) hintPrompt.SetActive(false);
    }

    void Update()
    {
        // Нажимаем E, только если игрок внутри и дверь сейчас НЕ движется
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E) && !isActivated)
        {
            StartCoroutine(ToggleDoor());
        }
    }

    IEnumerator ToggleDoor()
    {
        isActivated = true; // Запрещаем нажимать, пока дверь едет
        if (hintPrompt != null) hintPrompt.SetActive(false);

        // 1. Если есть что взрывать — взрываем (только при первом открытии или всегда, по желанию)
        if (objectToShatter != null)
        {
            objectToShatter.Shatter();
            objectToShatter = null; // Обнуляем, чтобы не пытаться взорвать уже взорванное
        }

        // 2. Определяем, куда ехать
        Vector3 targetPos = isOpen ? startPosition : endPosition;
        
        // 3. Двигаем дверь
        if (door != null)
        {
            while (Vector3.Distance(door.transform.position, targetPos) > 0.01f)
            {
                door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            door.transform.position = targetPos;
        }

        // 4. Меняем состояние
        isOpen = !isOpen; 
        isActivated = false; // Кнопка снова готова!

        // Показываем подсказку снова, если игрок всё еще в триггере
        if (isPlayerInside && hintPrompt != null) hintPrompt.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.StartsWith("Player") && !isActivated)
        {
            isPlayerInside = true;
            if (hintPrompt != null) hintPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.StartsWith("Player"))
        {
            isPlayerInside = false;
            if (hintPrompt != null) hintPrompt.SetActive(false);
        }
    }
}