using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movement")]
    public float minMoveSpeed = 0.2f;
    public float maxMoveSpeed = 1.0f;

    [Header("Rotation")]
    public float minRotationSpeed = 10f;
    public float maxRotationSpeed = 60f;

    [Header("Drift")]
    public float driftChangeStrength = 0.2f;

    private float moveSpeed;
    private float rotationSpeed;
    private Vector3 moveDirection;
    private Vector3 rotationAxis;

    void Start()
    {
        // ALWAYS move roughly toward camera (fixes disappearing)
        Vector3 baseDirection = -Camera.main.transform.forward;

        // Add slight randomness so it's not perfectly straight
        moveDirection = (baseDirection + Random.insideUnitSphere * 0.3f).normalized;

        // Random speeds
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        // Random rotation axis
        rotationAxis = Random.onUnitSphere;

        // Better scale range (avoid tiny invisible asteroids)
        float scale = Random.Range(1f, 3f);
        transform.localScale = Vector3.one * scale;
    }

    void Update()
    {
        // Slight drifting over time (optional realism)
        moveDirection += Random.insideUnitSphere * driftChangeStrength * Time.deltaTime;
        moveDirection.Normalize();

        // Move asteroid
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotate asteroid
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}