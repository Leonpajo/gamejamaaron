using UnityEngine;

/// <summary>
/// Attach this script to your island GameObject to make it gently bob up and down,
/// simulating a floating island on water.
/// </summary>
public class Islandbob : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("How high the island rises and falls from its starting position (in world units).")]
    public float amplitude = 0.3f;

    [Tooltip("How many full bobs per second. Lower = slower, more gentle.")]
    public float frequency = 0.4f;

    [Header("Tilt Settings")]
    [Tooltip("Enable a subtle rocking/tilting effect along with the bobbing.")]
    public bool enableTilt = true;

    [Tooltip("Maximum tilt angle in degrees on the X axis.")]
    public float tiltAmountX = 1.5f;

    [Tooltip("Maximum tilt angle in degrees on the Z axis.")]
    public float tiltAmountZ = 1.0f;

    [Tooltip("Speed multiplier for the tilt oscillation (relative to bob frequency).")]
    public float tiltFrequencyMultiplier = 0.7f;

    [Header("Phase Offset")]
    [Tooltip("Randomise the starting phase so multiple islands don't all bob in sync.")]
    public bool randomisePhaseOnStart = true;

    // Internal state
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _phaseOffset;

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        _phaseOffset = randomisePhaseOnStart ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void Update()
    {
        float time = Time.time * frequency * Mathf.PI * 2f + _phaseOffset;

        // --- Vertical bobbing ---
        float newY = _startPosition.y + Mathf.Sin(time) * amplitude;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);

        // --- Subtle tilt ---
        if (enableTilt)
        {
            float tiltTime = time * tiltFrequencyMultiplier;

            float tiltX = Mathf.Sin(tiltTime) * tiltAmountX;
            float tiltZ = Mathf.Cos(tiltTime * 1.3f) * tiltAmountZ; // slightly offset for natural feel

            Quaternion tiltRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
            transform.rotation = _startRotation * tiltRotation;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draws a preview of the bob range in the Scene view.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? _startPosition : transform.position;

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f);
        Gizmos.DrawLine(
            origin + Vector3.up * amplitude,
            origin - Vector3.up * amplitude
        );

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        Gizmos.DrawWireSphere(origin + Vector3.up * amplitude, 0.1f);
        Gizmos.DrawWireSphere(origin - Vector3.up * amplitude, 0.1f);
    }
#endif
}