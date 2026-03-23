using System.Collections;
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

    [Header("Transition")]
    public float transitionDuration = 1f;

    private bool _is2D = false;
    private bool _transitioning = false;
    private float _startDelay = 1f;
    private PlayerMovement2D _movement2D;
    private PlayerCam _playerCam;

    private void Update()
    {
        if (_startDelay > 0f)
            _startDelay -= Time.deltaTime;

        if (_is2D && !_transitioning)
        {
            // Lock Z and follow player
            Vector3 pos = player.position;
            pos.z = 0f;
            player.position = pos;

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
        if (_is2D || _transitioning) return;

        Activate2DMode(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!_is2D || _transitioning) return;

        Deactivate2DMode();
    }

    private void Activate2DMode(Collider other)
    {
        _playerCam = playerCamera.GetComponent<PlayerCam>();

        _movement2D = other.gameObject.GetComponent<PlayerMovement2D>();
        if (_movement2D == null)
            _movement2D = other.gameObject.AddComponent<PlayerMovement2D>();

        _movement2D.whatIsGround = playerController.whatIsGround;
        _movement2D.playerHeight = playerController.playerHeight;

        StartCoroutine(TransitionTo2D(other));
    }

    private void Deactivate2DMode()
    {
        StartCoroutine(TransitionTo3D());
    }

    private IEnumerator TransitionTo2D(Collider other)
    {
        _transitioning = true;

        // Disable controls during transition
        if (_playerCam != null) _playerCam.enabled = false;
        if (playerController != null) playerController.enabled = false;
        if (moveCameraScript != null) moveCameraScript.enabled = false;

        float elapsed = 0f;
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        float startFOV = playerCamera.fieldOfView;

        Vector3 targetPos = new Vector3(player.position.x, cameraHeight, cameraZOffset);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            // Keep target X in sync with player during transition
            targetPos.x = player.position.x;

            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, Quaternion.identity, t);
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, 60f, t);

            yield return null;
        }

        // Snap to final values
        playerCamera.orthographic = true;
        playerCamera.orthographicSize = orthographicSize;

        if (playerRigidbody != null)
            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        _is2D = true;
        _transitioning = false;
        _movement2D.enabled = true;
    }

    private IEnumerator TransitionTo3D()
    {
        _transitioning = true;

        if (_movement2D != null) _movement2D.enabled = false;

        // Reset Z before transitioning
        Vector3 pos = player.position;
        pos.z = transform.position.z;
        player.position = pos;

        // Switch back to perspective
        playerCamera.orthographic = false;

        float elapsed = 0f;
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        Vector3 targetPos = cameraPos != null ? cameraPos.position : player.position + Vector3.up * 0.5f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            // Keep target in sync with player
            if (cameraPos != null) targetPos = cameraPos.position;

            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, Quaternion.identity, t);

            yield return null;
        }

        // Snap final
        if (cameraHolder != null && cameraPos != null)
        {
            cameraHolder.position = cameraPos.position;
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        if (playerRigidbody != null)
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (moveCameraScript != null) moveCameraScript.enabled = true;
        if (_playerCam != null) _playerCam.enabled = true;
        if (playerController != null) playerController.enabled = true;

        _is2D = false;
        _transitioning = false;
    }
}