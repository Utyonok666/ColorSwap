using UnityEngine;

public class ColorBouncer : MonoBehaviour
{
    public enum BouncerColor { Red, Yellow, Green }
    public BouncerColor bouncerColor;
    public float bounceForce = 15f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Player"))
        {
            bool match = collision.gameObject.tag.Contains(bouncerColor.ToString());
            if (!match)
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 bounceDir = (collision.transform.position - transform.position).normalized;
                    rb.linearVelocity = bounceDir * bounceForce;
                }
            }
        }
    }
}