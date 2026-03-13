using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 LastCheckPointPos = Vector3.zero;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("CPX"))
        {
            LastCheckPointPos = new Vector3(
                PlayerPrefs.GetFloat("CPX"),
                PlayerPrefs.GetFloat("CPY"),
                PlayerPrefs.GetFloat("CPZ")
            );
        }
        else
        {
            LastCheckPointPos = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerRed") || 
            other.CompareTag("PlayerYellow") || other.CompareTag("PlayerGreen"))
        {
            LastCheckPointPos = transform.position;

            PlayerPrefs.SetFloat("CPX", transform.position.x);
            PlayerPrefs.SetFloat("CPY", transform.position.y);
            PlayerPrefs.SetFloat("CPZ", transform.position.z);
            PlayerPrefs.Save(); 

            Debug.Log("Чекпоинт сохранен!");
        }
    }
}