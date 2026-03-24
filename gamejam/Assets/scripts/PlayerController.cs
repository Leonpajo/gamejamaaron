using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    bool isSprinting;
    float moveSpeed;
    Vector3 moveDirection;
    Rigidbody rb;

    [HideInInspector] public bool is2D = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (is2D)
            Handle2DInput();
        else
            MyInput();

        SpeedControl();

        rb.linearDamping = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        if (is2D)
            Move2D();
        else
            MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(sprintKey);
        moveSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Handle2DInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = 0f;
        moveSpeed = walkSpeed;

        if (Input.GetKeyDown(jumpKey) && grounded && readyToJump)
        {
            readyToJump = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            Invoke(nameof(ResetJump), 0.2f);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!grounded && moveDirection != Vector3.zero)
        {
            if (Physics.Raycast(transform.position, moveDirection.normalized, 0.6f, whatIsGround))
                return;
        }

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void Move2D()
    {
        rb.AddForce(new Vector3(horizontalInput * moveSpeed * 10f, 0f, 0f), ForceMode.Force);

        float xSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (xSpeed > moveSpeed)
        {
            rb.linearVelocity = new Vector3(
                Mathf.Sign(rb.linearVelocity.x) * moveSpeed,
                rb.linearVelocity.y,
                0f);
        }
    }

    private void SpeedControl()
    {
        if (is2D) return;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}