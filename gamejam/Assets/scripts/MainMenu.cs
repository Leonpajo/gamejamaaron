using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Buttons")]
    [SerializeField] private RectTransform playButton;
    [SerializeField] private RectTransform quitButton;

    [Header("Animation")]
    [SerializeField] private float slideDistance = 800f;   // how far off-screen they start
    [SerializeField] private float playDuration = 0.55f;
    [SerializeField] private float quitDuration = 0.45f;
    [SerializeField] private float quitDelay = 0.12f;  // quit slides in slightly after play

    // Store the on-screen (target) positions set in the editor
    private Vector2 _playTarget;
    private Vector2 _quitTarget;

    private void Awake()
    {
        // Capture the positions you set in the Canvas
        _playTarget = playButton.anchoredPosition;
        _quitTarget = quitButton.anchoredPosition;

        // Move both buttons off the left edge before the scene shows
        playButton.anchoredPosition = new Vector2(_playTarget.x - slideDistance, _playTarget.y);
        quitButton.anchoredPosition = new Vector2(_quitTarget.x - slideDistance, _quitTarget.y);
    }

    private void Start()
    {
        StartCoroutine(AnimateIn(playButton, _playTarget, playDuration, 0f));
        StartCoroutine(AnimateIn(quitButton, _quitTarget, quitDuration, quitDelay));
    }

    // Smooth slide using an ease-out cubic curve
    private IEnumerator AnimateIn(RectTransform rt, Vector2 target, float duration, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        Vector2 startPos = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);   // ease-out cubic
            rt.anchoredPosition = Vector2.LerpUnclamped(startPos, target, eased);
            yield return null;
        }

        rt.anchoredPosition = target;
    }

    // ── Button callbacks ──────────────────────────────────────────────────────

    public void OnPlayPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}