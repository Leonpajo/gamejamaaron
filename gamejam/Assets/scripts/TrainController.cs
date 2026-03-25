using UnityEngine;

/// <summary>
/// Attach to the Train GameObject.
/// Add a Rigidbody and a SphereCollider (Is Trigger) to the train.
/// Press E near the train to delete the player and start driving.
/// </summary>
public class TrainController : MonoBehaviour
{
    [Header("Train Movement")]
    public float acceleration = 15f;
    public float maxSpeed = 20f;
    public float turnSpeed = 60f;

    [Header("Physics")]
    [Tooltip("Lower = tips easier, more negative = more stable")]
    public float centerOfMassY = -0.5f;

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

        // Allow tipping forward/back but not sideways roll
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

        // Set center of mass low so it tips naturally when unbalanced
        rb.centerOfMass = new Vector3(0f, centerOfMassY, 0f);

        if (trainCamera != null)
            trainCamera.enabled = false;
    }

    private void Update()
    {
        if (_playerInRange && !_isDriving && Input.GetKeyDown(interactKey))
            StartDriving();
    }

    private void FixedUpdate()
    {
        if (!_isDriving) return;

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.AddForce(-transform.forward * vertical * acceleration, ForceMode.Acceleration);

        // Clamp speed
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }

        // Slow down when no input
        if (vertical == 0)
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);

        // Turn only when moving
        if (flatVel.magnitude > 0.5f)
        {
            float turn = horizontal * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
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

        // Delete the player
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
}