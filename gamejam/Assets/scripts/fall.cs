using UnityEngine;

public class FallDeath : MonoBehaviour
{
    public float deathY = -10f;

    void Update()
    {
        if (transform.position.y < deathY)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died from falling");
        // Example: restart level
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}