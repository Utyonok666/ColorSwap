using UnityEngine;

// Changes the wall color and updates its behavior based on the current player
// Меняет цвет стены и обновляет её поведение в зависимости от текущего игрока
public class WallColorChanger : MonoBehaviour
{
    [Header("Colors: 0-Red, 1-Yellow, 2-Green")]
    public Color[] colors = { Color.red, Color.yellow, Color.green }; // Wall colors // Цвета стен
    public KeyCode changeKey = KeyCode.G; // Key to cycle colors // Кнопка для смены цвета

    private SpriteRenderer sr; // For visual color change // Для визуального изменения цвета
    private PlatformColor pc;  // For game logic (passable platforms) // Для игровой логики (проходимость платформ)
    private int index = 0;     // Current color index // Текущий индекс цвета

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PlatformColor>();
        UpdateWall(); // Initialize wall // Инициализируем стену
    }

    void Update()
    {
        if (Input.GetKeyDown(changeKey))
        {
            index = (index + 1) % colors.Length; // Cycle through colors // Переключаем цвета по кругу
            UpdateWall();
        }
    }

    void UpdateWall()
    {
        // 1. Update platform logic first (passable color)
        // 1. Сначала обновляем логику платформы (проходимый цвет)
        if (pc != null) pc.platformColor = (PlatformColor.ColorType)index;

        // 2. Update visual color (RGB) without changing transparency
        // 2. Меняем визуальный цвет (RGB), сохраняя текущую прозрачность
        if (sr != null && colors.Length > index)
        {
            float currentAlpha = sr.color.a;
            Color newCol = colors[index];
            sr.color = new Color(newCol.r, newCol.g, newCol.b, currentAlpha);
        }

        // 3. Notify platform manager to update passable states
        // 3. Уведомляем менеджер платформ, чтобы обновить состояния
        GameObject player = GameObject.FindGameObjectWithTag("PlayerRed") ?? 
                            GameObject.FindGameObjectWithTag("PlayerYellow") ?? 
                            GameObject.FindGameObjectWithTag("PlayerGreen");

        if (player != null && PlatformManager.Instance != null)
        {
            PlatformManager.Instance.NotifyColorChange(player.tag);
        }
    }
}