using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Singleton that controls the dialogue box UI.
/// Place this on a persistent GameObject in your scene alongside the canvas setup.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The root panel of the dialogue box (the dark bar at the bottom).")]
    public GameObject dialoguePanel;

    [Tooltip("TextMeshPro text for the NPC's name.")]
    public TextMeshProUGUI nameText;

    [Tooltip("TextMeshPro text for the dialogue body.")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("Small animated icon shown while text is typing (e.g. blinking dots).")]
    public GameObject typingIndicator;

    [Tooltip("The 'Press E to continue' prompt shown between lines.")]
    public GameObject continuePrompt;

    [Header("Typewriter Settings")]
    [Tooltip("Characters revealed per second.")]
    public float typingSpeed = 40f;

    // Internal state
    private string[] _lines;
    private int _currentLine;
    private bool _isTyping;
    private bool _skipRequested;
    private Coroutine _typingCoroutine;

    // Callback fired when the full conversation ends
    private System.Action _onDialogueEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        CloseDialogue();
    }

    // ------------------------------------------------------------------ public API

    /// <summary>
    /// Called by NPCInteractable to start a conversation.
    /// </summary>
    public void StartDialogue(DialogueData data, System.Action onEnd = null)
    {
        if (data == null || data.lines == null || data.lines.Length == 0) return;

        _lines = data.lines;
        _currentLine = 0;
        _onDialogueEnd = onEnd;

        nameText.text = data.npcName;
        dialoguePanel.SetActive(true);
        ShowLine(_currentLine);
    }

    /// <summary>
    /// Called each frame by the player / NPC when the interact key is pressed.
    /// </summary>
    public void Advance()
    {
        if (!dialoguePanel.activeSelf) return;

        if (_isTyping)
        {
            // Skip the typewriter animation — show full line immediately
            _skipRequested = true;
            return;
        }

        _currentLine++;
        if (_currentLine < _lines.Length)
        {
            ShowLine(_currentLine);
        }
        else
        {
            CloseDialogue();
        }
    }

    public bool IsOpen => dialoguePanel != null && dialoguePanel.activeSelf;

    // ------------------------------------------------------------------ internals

    private void ShowLine(int index)
    {
        if (continuePrompt) continuePrompt.SetActive(false);
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeLine(_lines[index]));
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        _skipRequested = false;
        dialogueText.text = "";

        if (typingIndicator) typingIndicator.SetActive(true);

        float delay = 1f / Mathf.Max(typingSpeed, 1f);

        foreach (char c in line)
        {
            if (_skipRequested) break;
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        // Ensure full line is always shown (handles skip)
        dialogueText.text = line;
        _isTyping = false;
        _skipRequested = false;

        if (typingIndicator) typingIndicator.SetActive(false);
        if (continuePrompt) continuePrompt.SetActive(true);
    }

    public void CloseDialogue()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _isTyping = false;

        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (typingIndicator) typingIndicator.SetActive(false);
        if (continuePrompt) continuePrompt.SetActive(false);
        if (dialogueText) dialogueText.text = "";

        _onDialogueEnd?.Invoke();
        _onDialogueEnd = null;
    }
}