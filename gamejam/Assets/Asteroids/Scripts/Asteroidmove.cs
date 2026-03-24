using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    public float speed = 2f;     // how fast it moves
    public float distance = 3f;  // how far it moves left/right

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPos + new Vector3(movement, 0, 0);
    }
}