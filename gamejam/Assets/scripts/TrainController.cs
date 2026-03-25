using UnityEngine;

/// <summary>
/// Attach to the Train GameObject.
/// </summary>
public class TrainController : MonoBehaviour
{
    [Header("Train Movement")]
    public float acceleration = 15f;
    public float maxSpeed = 20f;
    public float turnSpeed = 60f;
    public float airMultiplier = 0.3f;

    [Header("Self Righting")]
    public float uprightTorque = 500f;
    public float uprightDamping = 5f;

    [Header("Center of Mass")]
    public float centerOfMassY = -0.5f;

    [Header("Ground Check")]
    public float trainHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("References")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;
    public TrainCamera trainCamera;

    private Rigidbody rb;
    private bool _isDriving = false;
    private bool _playerInRange = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Allow full physics rotation (no freezing now)
        rb.constraints = RigidbodyConstraints.None;

        rb.centerOfMass = new Vector3(0f, centerOfMassY, 0f);

        if (trainCamera != null)
            trainCamera.enabled = false;
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, trainHeight * 0.5f + 0.3f, whatIsGround);

        if (_playerInRange && !_isDriving && Input.GetKeyDown(interactKey))
            StartDriving();
    }

    private void FixedUpdate()
    {
        if (!_isDriving) return;

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        // Movement
        if (grounded)
        {
            rb.AddForce(-transform.forward * vertical * acceleration, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(-transform.forward * vertical * acceleration * airMultiplier, ForceMode.Acceleration);
        }

        // Speed cap
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }

        // Braking
        if (vertical == 0)
        {
            Vector3 braked = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, braked, Time.fixedDeltaTime * 2f);
        }

        // Turning
        if (flatVel.magnitude > 0.5f)
        {
            float turn = horizontal * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }

        // 🔥 SELF-RIGHTING SYSTEM (natural fall back)
        Vector3 currentUp = transform.up;
        Vector3 targetUp = Vector3.up;

        Vector3 torqueAxis = Vector3.Cross(currentUp, targetUp);
        float angle = Vector3.Angle(currentUp, targetUp);
        float strength = angle / 180f;

        rb.AddTorque(torqueAxis * strength * uprightTorque, ForceMode.Acceleration);

        // Damping (prevents wobble)
        rb.angularVelocity *= (1f - uprightDamping * Time.fixedDeltaTime);
    }

    private void StartDriving()
    {
        _isDriving = true;

        if (trainCamera != null)
        {
            trainCamera.enabled = true;

            var playerCam = trainCamera.GetComponent<PlayerCam>();
            if (playerCam != null) playerCam.enabled = false;

            var moveCamera = trainCamera.GetComponent<MoveCamera>();
            if (moveCamera != null) moveCamera.enabled = false;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
            Destroy(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = false;
    }

    private void OnDrawGizmos()
    {
        // Center of mass
        Vector3 comWorld = transform.TransformPoint(new Vector3(0f, centerOfMassY, 0f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(comWorld, 0.2f);
        Gizmos.DrawLine(transform.position, comWorld);

        // Ground ray
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,
            transform.position + Vector3.down * (trainHeight * 0.5f + 0.3f));

#if UNITY_EDITOR
        UnityEditor.Handles.Label(comWorld + Vector3.up * 0.3f, "Center of Mass");
#endif
    }
}