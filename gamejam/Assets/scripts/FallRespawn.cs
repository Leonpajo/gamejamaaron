using UnityEngine;

/// <summary>
/// Attach to the Player. If player falls below deathY, respawns at spawnPoint.
/// </summary>
public class FallRespawn : MonoBehaviour
{
    [Tooltip("Y position below which the player respawns")]
    public float deathY = -20f;

    [Tooltip("The current spawn position")]
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

    public void SetSpawnPoint(Transform newSpawnPoint) // 👈 NEW
    {
        spawnPoint = newSpawnPoint;
    }

    private void Respawn()
    {
        if (spawnPoint == null) return;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        transform.position = spawnPoint.position;
    }
}