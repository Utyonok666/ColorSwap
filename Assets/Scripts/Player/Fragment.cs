using UnityEngine;

public class Fragment : MonoBehaviour
{
    private SpriteRenderer sr;
    private Vector3 startOffset;
    private Vector3 targetDir;
    private float speed;
    private float fadeSpeed;
    private bool isReforming;
    private float timer = 0;

    public void Init(Sprite sprite, Color color, Vector3 dir, float moveSpeed, float fade, float size, bool reform)
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = 10;
        
        speed = moveSpeed;
        fadeSpeed = fade;
        isReforming = reform;
        transform.localScale = Vector3.one * size;

        if (isReforming) {
            startOffset = dir * speed * 0.4f;
            transform.localPosition = startOffset;
            sr.color = new Color(color.r, color.g, color.b, 0); 
        } else {
            targetDir = dir;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (isReforming) {
            transform.localPosition = Vector3.Lerp(startOffset, Vector3.zero, timer * speed * 0.8f);
            Color c = sr.color;
            c.a += fadeSpeed * Time.deltaTime;
            sr.color = c;
            if (timer > 1.2f || Vector3.Distance(transform.localPosition, Vector3.zero) < 0.05f) Destroy(gameObject);
        } else {
            transform.position += targetDir * speed * Time.deltaTime;
            transform.Rotate(0, 0, 200 * Time.deltaTime);
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;
            if (c.a <= 0) Destroy(gameObject);
        }
    }
}