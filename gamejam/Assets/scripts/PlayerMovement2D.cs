using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("2D Movement")]
    public float moveSpeed = 4f;
    public float jumpForce = 6f;
    public float groundDrag = 5f;
    public KeyCode jumpKey = KeyCode.Space;
    public LayerMask whatIsGround;
    public float playerHeight = 2f;

    private Rigidbody rb;
    private bool grounded;
    private bool _wasGrounded;
    private bool readyToJump = true;
    private bool _jumped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enabled = false;
    }

    private void OnEnable()
    {
        readyToJump = true;
        _jumped = false;
    }

    private void Update()
    {
        _wasGrounded = grounded;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        // Reset jumped when landing
        if (grounded && _jumped)
            _jumped = false;

        rb.linearDamping = grounded ? groundDrag : 0f;

        if (Input.GetKeyDown(jumpKey) && grounded && readyToJump && !_jumped)
        {
            _jumped = true;
            readyToJump = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            Invoke(nameof(ResetJump), 0.8f);
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.AddForce(new Vector3(horizontal * moveSpeed * 10f, 0f, 0f), ForceMode.Force);

        float xSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (xSpeed > moveSpeed)
        {
            rb.linearVelocity = new Vector3(
                Mathf.Sign(rb.linearVelocity.x) * moveSpeed,
                rb.linearVelocity.y,
                0f);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}