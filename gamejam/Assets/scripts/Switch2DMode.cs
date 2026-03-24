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
    public float cameraHeight = 2f;
    public float cameraZOffset = -10f;
    public float orthographicSize = 5f;

    [Header("Transition")]
    public float transitionDuration = 1f;

    private bool _is2D = false;
    private bool _transitioning = false;
    private float _startDelay = 1f;
    private PlayerCam _playerCam;

    private void Update()
    {
        if (_startDelay > 0f)
            _startDelay -= Time.deltaTime;

        if (_is2D && !_transitioning)
        {
            // Lock Z
            Vector3 pos = player.position;
            pos.z = 0f;
            player.position = pos;

            // Camera follows player X and Y
            Vector3 camPos = playerCamera.transform.position;
            camPos.x = player.position.x;
            camPos.y = player.position.y + cameraHeight;
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

        StartCoroutine(TransitionTo2D());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!_is2D || _transitioning) return;

        StartCoroutine(TransitionTo3D());
    }

    private IEnumerator TransitionTo2D()
    {
        _transitioning = true;

        _playerCam = playerCamera.GetComponent<PlayerCam>();
        if (_playerCam != null) _playerCam.enabled = false;
        if (moveCameraScript != null) moveCameraScript.enabled = false;

        float elapsed = 0f;
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        float startFOV = playerCamera.fieldOfView;
        Vector3 targetPos = new Vector3(player.position.x, player.position.y + cameraHeight, cameraZOffset);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            targetPos.x = player.position.x;
            targetPos.y = player.position.y + cameraHeight;

            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, Quaternion.identity, t);
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, 60f, t);

            yield return null;
        }

        playerCamera.orthographic = true;
        playerCamera.orthographicSize = orthographicSize;

        if (playerRigidbody != null)
            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        playerController.is2D = true;

        _is2D = true;
        _transitioning = false;
    }

    private IEnumerator TransitionTo3D()
    {
        _transitioning = true;

        playerController.is2D = false;

        if (playerRigidbody != null)
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // Reset Z
        Vector3 pos = player.position;
        pos.z = transform.position.z;
        player.position = pos;

        playerCamera.orthographic = false;

        float elapsed = 0f;
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        Vector3 targetPos = cameraPos != null ? cameraPos.position : player.position + Vector3.up * 0.5f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            if (cameraPos != null) targetPos = cameraPos.position;

            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, Quaternion.identity, t);

            yield return null;
        }

        if (cameraHolder != null && cameraPos != null)
        {
            cameraHolder.position = cameraPos.position;
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        if (moveCameraScript != null) moveCameraScript.enabled = true;
        if (_playerCam != null) _playerCam.enabled = true;

        _is2D = false;
        _transitioning = false;
    }
    public void ForceReset3D()
    {
        StopAllCoroutines();

        _is2D = false;
        _transitioning = false;

        playerCamera.orthographic = false;

        if (playerRigidbody != null)
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (cameraHolder != null && cameraPos != null)
        {
            cameraHolder.position = cameraPos.position;
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        if (moveCameraScript != null) moveCameraScript.enabled = true;
        if (_playerCam != null) _playerCam.enabled = true;
    }
}