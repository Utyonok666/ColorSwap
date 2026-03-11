using UnityEngine;

public class PlatformColor : MonoBehaviour
{
    public enum ColorType { Red, Yellow, Green } 
    [Header("Цвет этой платформы")]
    public ColorType platformColor;
    public float transparentAlpha = 0.3f;

    private SpriteRenderer sr;
    private int originalLayer;
    private int passableLayer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalLayer = gameObject.layer; 
        passableLayer = LayerMask.NameToLayer("PassablePlatform");
    }

    public void UpdateState(string playerTag)
    {
        string myColorName = platformColor.ToString();
        bool match = playerTag.Contains(myColorName);

        if (sr == null) return;

        if (match)
        {
            gameObject.layer = passableLayer;
            // Сохраняем текущий RGB, меняем только A (прозрачность)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparentAlpha);
        }
        else
        {
            gameObject.layer = originalLayer;
            // Сохраняем текущий RGB, возвращаем полную видимость
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        }
    }
}