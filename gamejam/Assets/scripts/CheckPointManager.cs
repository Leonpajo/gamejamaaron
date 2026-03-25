using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    private Transform currentCheckpoint;

    private void Awake()
    {
        instance = this;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint saved: " + newCheckpoint.name);
    }

    public void Respawn(GameObject player)
    {
        if (currentCheckpoint != null)
        {
            player.transform.position = currentCheckpoint.position;
            player.transform.rotation = currentCheckpoint.rotation;
        }
        else
        {
            Debug.LogWarning("No checkpoint set!");
        }
    }
}