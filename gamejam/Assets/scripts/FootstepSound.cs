using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public PlayerController player;
    public AudioClip footstepClip;
    public float footstepInterval = 0.4f; // time between steps

    private AudioSource audioSource;
    private float timer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (player.grounded && isMoving)
        {
            timer += Time.deltaTime;
            if (timer >= footstepInterval)
            {
                audioSource.PlayOneShot(footstepClip);
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }
    }
}