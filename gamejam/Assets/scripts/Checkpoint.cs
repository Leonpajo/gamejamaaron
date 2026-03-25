using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FallRespawn fallRespawn = other.GetComponent<FallRespawn>();
        if (fallRespawn != null)
        {
            fallRespawn.SetSpawnPoint(spawnPoint);
            Debug.Log("Checkpoint set to: " + spawnPoint.name);
        }
    }
}