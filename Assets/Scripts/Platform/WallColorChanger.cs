using UnityEngine;

public class WallColorChanger : MonoBehaviour
{
    [Header("Цвета: 0-Red, 1-Yellow, 2-Green")]
    public Color[] colors = { Color.red, Color.yellow, Color.green };
    public KeyCode changeKey = KeyCode.G;

    private SpriteRenderer sr;
    private PlatformColor pc;
    private int index = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PlatformColor>();
        UpdateWall();
    }

    void Update()
    {
        if (Input.GetKeyDown(changeKey))
        {
            index = (index + 1) % colors.Length;
            UpdateWall();
        }
    }

    void UpdateWall()
    {
        // 1. Сначала меняем логику цвета
        if (pc != null) pc.platformColor = (PlatformColor.ColorType)index;

        // 2. Затем меняем сам цвет (RGB)
        if (sr != null && colors.Length > index)
        {
            // Берем цвет из массива, но сохраняем текущую прозрачность, чтобы не было "мигания"
            float currentAlpha = sr.color.a;
            Color newCol = colors[index];
            sr.color = new Color(newCol.r, newCol.g, newCol.b, currentAlpha);
        }

        // 3. Обновляем состояние через менеджер
        GameObject player = GameObject.FindGameObjectWithTag("PlayerRed") ?? 
                            GameObject.FindGameObjectWithTag("PlayerYellow") ?? 
                            GameObject.FindGameObjectWithTag("PlayerGreen");

        if (player != null && PlatformManager.Instance != null)
        {
            PlatformManager.Instance.NotifyColorChange(player.tag);
        }
    }
}