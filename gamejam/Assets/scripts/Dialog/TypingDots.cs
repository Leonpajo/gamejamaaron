using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the TypingIndicator GameObject.
/// Add three child Image objects (small white dots) and assign them below.
/// They will pulse in sequence to show the NPC is "thinking".
/// </summary>
public class TypingDots : MonoBehaviour
{
    [Tooltip("The three dot Image components (in order).")]
    public Image[] dots = new Image[3];

    [Tooltip("Seconds between each dot lighting up.")]
    public float stepDelay = 0.25f;

    [Tooltip("Alpha when a dot is 'off'.")]
    [Range(0f, 1f)]
    public float dimAlpha = 0.2f;

    private Coroutine _anim;

    private void OnEnable()
    {
        _anim = StartCoroutine(AnimateDots());
    }

    private void OnDisable()
    {
        if (_anim != null) StopCoroutine(_anim);
        foreach (var d in dots)
            if (d) SetAlpha(d, dimAlpha);
    }

    private IEnumerator AnimateDots()
    {
        int i = 0;
        while (true)
        {
            foreach (var d in dots) if (d) SetAlpha(d, dimAlpha);
            if (i < dots.Length && dots[i]) SetAlpha(dots[i], 1f);
            i = (i + 1) % dots.Length;
            yield return new WaitForSeconds(stepDelay);
        }
    }

    private static void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}