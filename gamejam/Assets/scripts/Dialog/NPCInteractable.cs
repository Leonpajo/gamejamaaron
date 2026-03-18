using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to any NPC GameObject.
/// Requires a Collider set to "Is Trigger" for the interaction zone.
/// Optionally add a child GameObject with a Canvas/Image as the interaction prompt (e.g. "[E] Talk").
/// </summary>
public class NPCInteractable : MonoBehaviour
{
    [Header("Dialogue")]
    [Tooltip("The dialogue data asset for this NPC (create via Assets > Create > Dialogue > Dialogue Data).")]
    public DialogueData dialogueData;

    [Header("Interaction")]
    [Tooltip("Key the player presses to start / advance dialogue.")]
    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Tag used to identify the player.")]
    public string playerTag = "Player";

    [Header("Prompt UI")]
    [Tooltip("Optional world-space prompt shown above the NPC when the player is in range (e.g. '[E] Talk').")]
    public GameObject interactPrompt;

    [Header("NPC Behaviour")]
    [Tooltip("Face the player when talking.")]
    public bool facePlayerWhenTalking = true;

    [Tooltip("Only rotate on the Y axis (keeps the NPC upright).")]
    public bool lockRotationToY = true;

    // State
    private bool _playerInRange;
    private Transform _playerTransform;
    private bool _inConversation;

    private void Start()
    {
        if (interactPrompt) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(interactKey))
        {
            if (_inConversation)
            {
                DialogueManager.Instance?.Advance();
            }
            else
            {
                StartConversation();
            }
        }

        // Face player during conversation
        if (_inConversation && facePlayerWhenTalking && _playerTransform != null)
        {
            FaceTarget(_playerTransform.position);
        }
    }

    // ------------------------------------------------------------------ triggers

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = true;
        _playerTransform = other.transform;
        if (interactPrompt) interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = false;
        if (interactPrompt) interactPrompt.SetActive(false);

        if (_inConversation)
        {
            _inConversation = false;
            DialogueManager.Instance?.CloseDialogue();
        }
    }

    // ------------------------------------------------------------------ internals

    private void StartConversation()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("NPCInteractable: No DialogueManager found in scene.");
            return;
        }

        if (interactPrompt) interactPrompt.SetActive(false);
        _inConversation = true;

        DialogueManager.Instance.StartDialogue(dialogueData, OnConversationEnd);
    }

    private void OnConversationEnd()
    {
        _inConversation = false;
        if (_playerInRange && interactPrompt)
            interactPrompt.SetActive(true);
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position);
        direction.y = 0f;
        direction.Normalize();
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90f, 180f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    // ------------------------------------------------------------------ gizmos

    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<SphereCollider>();
        if (col == null) return;
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, col.radius);
    }
}