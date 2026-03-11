using UnityEngine;
using System.Collections; 

public class RotatingBouncer : MonoBehaviour
{
    public float rotationSpeed = 60f; 
    public float bounceForceX = 10f; 
    public float bounceForceY = 8f; 
    public float controlDisableTime = 0.3f; 
    
    private Rigidbody2D _rb;
    private bool _isBouncing = false; 

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null) {
            _rb.bodyType = RigidbodyType2D.Kinematic; 
            _rb.useFullKinematicContacts = true;
        }
    }

    private void FixedUpdate()
    {
        _rb.MoveRotation(_rb.rotation + rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Player") || collision.gameObject.CompareTag("PlayerRed") || 
            collision.gameObject.CompareTag("PlayerYellow") || collision.gameObject.CompareTag("PlayerGreen"))
        {
            if (!_isBouncing)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                Move playerMove = collision.gameObject.GetComponent<Move>(); 
                if (playerRb != null && playerMove != null) StartCoroutine(ApplyBounce(playerRb, playerMove));
            }
        }
    }

    private IEnumerator ApplyBounce(Rigidbody2D playerRb, Move playerMove)
    {
        _isBouncing = true; 
        playerMove.SetInputState(false);

        float side = playerRb.position.x < transform.position.x ? -1 : 1;
        playerRb.linearVelocity = Vector2.zero; 
        playerRb.AddForce(new Vector2(bounceForceX * side, bounceForceY), ForceMode2D.Impulse);

        yield return new WaitForSeconds(controlDisableTime);
        playerMove.SetInputState(true);
        _isBouncing = false; 
    }
}