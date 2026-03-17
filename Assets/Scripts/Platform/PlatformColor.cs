using UnityEngine;

// Platform that changes passability and transparency based on player's color
// Платформа, которая меняет проходимость и прозрачность в зависимости от цвета игрока
public class PlatformColor : MonoBehaviour
{
    public enum ColorType { Red, Yellow, Green } // Possible platform colors // Возможные цвета платформы

    [Header("This platform's color")]
    public ColorType platformColor;   // Current color of platform // Цвет этой платформы
    public float transparentAlpha = 0.3f; // Alpha when passable // Прозрачность при проходимости

    private SpriteRenderer sr;   // Renderer to change color // Рендер для изменения цвета
    private int originalLayer;   // Layer when solid // Исходный слой
    private int passableLayer;   // Layer when passable // Слой при проходимости

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalLayer = gameObject.layer; 
        passableLayer = LayerMask.NameToLayer("PassablePlatform"); // Set passable layer // Слой "проходимой платформы"
    }

    // Updates the platform state according to player's tag
    // Обновляет состояние платформы в зависимости от тега игрока
    public void UpdateState(string playerTag)
    {
        string myColorName = platformColor.ToString(); // Convert enum to string // Преобразуем enum в строку
        bool match = playerTag.Contains(myColorName);  // Check if player's color matches // Проверка цвета игрока

        if (sr == null) return;

        if (match)
        {
            gameObject.layer = passableLayer; // Make platform passable // Делаем платформу проходимой
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparentAlpha); // Change only alpha // Меняем только прозрачность
        }
        else
        {
            gameObject.layer = originalLayer; // Solid layer // Возвращаем исходный слой
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f); // Fully visible // Полная видимость
        }
    }
}