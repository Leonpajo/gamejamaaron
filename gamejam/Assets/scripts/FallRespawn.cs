using UnityEngine;

/// <summary>
/// Attach to the Player. If player falls below deathY, respawns at spawnPoint.
/// Create an empty GameObject in your scene and assign it as the spawnPoint.
/// </summary>
public class FallRespawn : MonoBehaviour
{
    [Tooltip("Y position below which the player respawns")]
    public float deathY = -20f;

    [Tooltip("The empty GameObject marking the spawn position")]
    public Transform spawnPoint;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < deathY)
            Respawn();
    }

    private void Respawn()
    {
        if (spawnPoint == null) return;

        // Stop all velocity before teleporting
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        transform.position = spawnPoint.position;
    }

}