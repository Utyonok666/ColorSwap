using UnityEngine;

// Applies a bounce force to the player if their colors don't match
// Применяет силу отталкивания к игроку, если их цвета не совпадают
public class ColorBouncer : MonoBehaviour
{
    public enum BouncerColor { Red, Yellow, Green }

    [Header("Settings / Настройки")]
    public BouncerColor bouncerColor; // The color assigned to this bouncer / Цвет, назначенный этому батуту
    public float bounceForce = 15f;   // Strength of the bounce / Сила отскока

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is the player (by tag)
        // Проверяем, является ли столкнувшийся объект игроком (по тегу)
        if (collision.gameObject.tag.StartsWith("Player"))
        {
            // Check if player's color tag matches the bouncer's color
            // Проверяем, совпадает ли цветовой тег игрока с цветом батута
            bool match = collision.gameObject.tag.Contains(bouncerColor.ToString());
            
            // If colors do NOT match, trigger the bounce effect
            // Если цвета НЕ совпадают, активируем эффект отскока
            if (!match)
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Calculate bounce direction away from the bouncer's center
                    // Вычисляем направление отскока от центра батута
                    Vector2 bounceDir = (collision.transform.position - transform.position).normalized;
                    
                    // Apply instantaneous velocity for the bounce
                    // Применяем мгновенную скорость для отскока
                    rb.linearVelocity = bounceDir * bounceForce;
                }
            }
        }
    }
}