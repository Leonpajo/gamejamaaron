using UnityEngine;

/// <summary>
/// Attach to the PlayerCam (or any camera GameObject).
/// Gets enabled by TrainController when player enters train.
/// </summary>
public class TrainCamera : MonoBehaviour
{
    [Header("References")]
    public Transform train;

    [Header("Settings")]
    public float distance = 8f;
    public float height = 3f;
    public float smoothSpeed = 5f;
    public float rotationSmoothSpeed = 3f;

    private void LateUpdate()
    {
        if (train == null) return;
        Vector3 targetPos = train.position
            + train.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // Look at the train smoothly
        Quaternion targetRot = Quaternion.LookRotation(train.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothSpeed);
    }
}