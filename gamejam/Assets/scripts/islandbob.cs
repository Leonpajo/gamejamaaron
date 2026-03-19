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

    [Header("X-Axis Drift Settings")]
    [Tooltip("Enable gentle side-to-side movement along the X axis.")]
    public bool enableXDrift = true;
    [Tooltip("How far the island drifts left and right from its starting position (in world units).")]
    public float xDriftAmplitude = 0.5f;
    [Tooltip("Speed multiplier for the X drift oscillation (relative to bob frequency).")]
    public float xDriftFrequencyMultiplier = 0.5f;

    [Header("Z-Axis Drift Settings")]
    [Tooltip("Enable gentle back-and-forth movement along the Z axis.")]
    public bool enableZDrift = true;
    [Tooltip("How far the island drifts forward and backward from its starting position (in world units).")]
    public float zDriftAmplitude = 0.5f;
    [Tooltip("Speed multiplier for the Z drift oscillation (relative to bob frequency).")]
    public float zDriftFrequencyMultiplier = 0.6f;

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

        // --- X-Axis Drift ---
        float newX = _startPosition.x;
        if (enableXDrift)
        {
            float xTime = time * xDriftFrequencyMultiplier;
            newX = _startPosition.x + Mathf.Cos(xTime) * xDriftAmplitude;
        }

        // --- Z-Axis Drift ---
        float newZ = _startPosition.z;
        if (enableZDrift)
        {
            float zTime = time * zDriftFrequencyMultiplier;
            newZ = _startPosition.z + Mathf.Sin(zTime) * zDriftAmplitude;
        }

        transform.position = new Vector3(newX, newY, newZ);

        // --- Subtle tilt ---
        if (enableTilt)
        {
            float tiltTime = time * tiltFrequencyMultiplier;
            float tiltX = Mathf.Sin(tiltTime) * tiltAmountX;
            float tiltZ = Mathf.Cos(tiltTime * 1.3f) * tiltAmountZ;
            Quaternion tiltRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
            transform.rotation = _startRotation * tiltRotation;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? _startPosition : transform.position;

        // Y bob range (blue)
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f);
        Gizmos.DrawLine(
            origin + Vector3.up * amplitude,
            origin - Vector3.up * amplitude
        );
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        Gizmos.DrawWireSphere(origin + Vector3.up * amplitude, 0.1f);
        Gizmos.DrawWireSphere(origin - Vector3.up * amplitude, 0.1f);

        // X drift range (green)
        if (enableXDrift)
        {
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.8f);
            Gizmos.DrawLine(
                origin + Vector3.right * xDriftAmplitude,
                origin - Vector3.right * xDriftAmplitude
            );
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.3f);
            Gizmos.DrawWireSphere(origin + Vector3.right * xDriftAmplitude, 0.1f);
            Gizmos.DrawWireSphere(origin - Vector3.right * xDriftAmplitude, 0.1f);
        }

        // Z drift range (orange)
        if (enableZDrift)
        {
            Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.8f);
            Gizmos.DrawLine(
                origin + Vector3.forward * zDriftAmplitude,
                origin - Vector3.forward * zDriftAmplitude
            );
            Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.3f);
            Gizmos.DrawWireSphere(origin + Vector3.forward * zDriftAmplitude, 0.1f);
            Gizmos.DrawWireSphere(origin - Vector3.forward * zDriftAmplitude, 0.1f);
        }
    }
#endif
}