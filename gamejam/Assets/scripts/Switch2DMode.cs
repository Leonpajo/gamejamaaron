using UnityEngine;

public class Switch2DMode : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera playerCamera;
    public PlayerController playerController;
    public Rigidbody playerRigidbody;
    public MoveCamera moveCameraScript;
    public Transform cameraHolder;
    public Transform cameraPos;

    [Header("2D Settings")]
    public float cameraHeight = 5f;
    public float cameraZOffset = -10f;
    public float orthographicSize = 5f;

    private bool _is2D = false;
    private float _startDelay = 1f;
    private PlayerMovement2D _movement2D;
    private PlayerCam _playerCam;

    private void Update()
    {
        if (_startDelay > 0f)
            _startDelay -= Time.deltaTime;

        if (_is2D)
        {
            // Lock Z position
            Vector3 pos = player.position;
            pos.z = 0f;
            player.position = pos;

            // Camera follows player on X axis only
            Vector3 camPos = playerCamera.transform.position;
            camPos.x = player.position.x;
            camPos.y = cameraHeight;
            camPos.z = cameraZOffset;
            playerCamera.transform.position = camPos;
            playerCamera.transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_startDelay > 0f) return;
        if (!other.CompareTag("Player")) return;
        if (_is2D) return;

        Activate2DMode(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!_is2D) return;

        Deactivate2DMode();
    }

    private void Activate2DMode(Collider other)
    {
        _is2D = true;

        playerCamera.orthographic = true;
        playerCamera.orthographicSize = orthographicSize;

        if (moveCameraScript != null)
            moveCameraScript.enabled = false;

        _playerCam = playerCamera.GetComponent<PlayerCam>();
        if (_playerCam != null)
            _playerCam.enabled = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ
                | RigidbodyConstraints.FreezeRotation;
        }

        if (playerController != null)
            playerController.enabled = false;

        _movement2D = other.gameObject.GetComponent<PlayerMovement2D>();
        if (_movement2D == null)
            _movement2D = other.gameObject.AddComponent<PlayerMovement2D>();

        _movement2D.whatIsGround = playerController.whatIsGround;
        _movement2D.playerHeight = playerController.playerHeight;
        _movement2D.enabled = true;
    }

    private void Deactivate2DMode()
    {
        _is2D = false;

        playerCamera.orthographic = false;

        // Snap CameraHolder back to player
        if (cameraHolder != null && cameraPos != null)
        {
            cameraHolder.position = cameraPos.position;
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        if (moveCameraScript != null)
            moveCameraScript.enabled = true;

        if (_playerCam != null)
            _playerCam.enabled = true;

        if (playerRigidbody != null)
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (playerController != null)
            playerController.enabled = true;

        if (_movement2D != null)
            _movement2D.enabled = false;

        // Reset player Z
        Vector3 pos = player.position;
        pos.z = transform.position.z;
        player.position = pos;
    }
}