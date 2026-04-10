using UnityEngine;

/// <summary>
/// Handles all LeanTween animations for the DuelMenu scene.
/// Attach to the duelMenuAnim GameObject and wire references in the Inspector.
/// AnimateIntro() is called automatically on Start.
/// Call the public methods from your game logic or Button OnClick events.
/// </summary>
public class duelMenuAnim : MonoBehaviour
{
    [Header("Header")]
    public RectTransform title;
    public RectTransform subtitle;        // "Kijou ?"

    [Header("Perso Buttons")]
    public RectTransform[] persoButtons;  // 4 buttons in order

    [Header("Action Buttons")]
    public RectTransform startButton;
    public RectTransform retourButton;

    [Header("Timing")]
    public float introDuration = 0.45f;
    public float staggerDelay = 0.08f;
    public float buttonBounceDuration = 0.13f;

    private const float OffscreenTop = 400f;
    private const float OffscreenLeft = -700f;

    private Vector2 titleOrigin;
    private Vector2 subtitleOrigin;
    private Vector2 retourOrigin;

    void Start()
    {
        CacheOrigins();
        AnimateIntro();
    }

    // -------------------------------------------------------------------------
    // Intro
    // -------------------------------------------------------------------------

    /// <summary>
    /// Plays the full entrance animation for all DuelMenu UI elements.
    /// </summary>
    public void AnimateIntro()
    {
        AnimateTitleIn();
        AnimateSubtitleIn();
        AnimatePersoButtonsIn();
        AnimateStartButtonIn();
        AnimateRetourButtonIn();
    }

    // -------------------------------------------------------------------------
    // Header
    // -------------------------------------------------------------------------

    private void AnimateTitleIn()
    {
        if (title == null) return;

        title.anchoredPosition = new Vector2(titleOrigin.x, titleOrigin.y + OffscreenTop);

        LeanTween.move(title, titleOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    private void AnimateSubtitleIn()
    {
        if (subtitle == null) return;

        subtitle.anchoredPosition = new Vector2(subtitleOrigin.x, subtitleOrigin.y + OffscreenTop);

        LeanTween.move(subtitle, subtitleOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(staggerDelay);
    }

    // -------------------------------------------------------------------------
    // Perso Buttons
    // -------------------------------------------------------------------------

    private void AnimatePersoButtonsIn()
    {
        if (persoButtons == null) return;

        for (int i = 0; i < persoButtons.Length; i++)
        {
            RectTransform btn = persoButtons[i];
            if (btn == null) continue;

            btn.localScale = Vector3.zero;

            LeanTween.scale(btn, Vector3.one, introDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(staggerDelay * (i + 2));
        }
    }

    /// <summary>
    /// Squeeze press on a PersoButton when clicked. Pass the button's index (0-3).
    /// </summary>
    public void AnimatePersoButtonPress(int index)
    {
        if (persoButtons == null || index < 0 || index >= persoButtons.Length) return;
        if (persoButtons[index] == null) return;

        AnimateButtonPress(persoButtons[index], Vector3.one);
    }

    /// <summary>
    /// Pops the selected PersoButton up and resets all others to scale 1.
    /// </summary>
    public void AnimatePersoButtonSelected(int selectedIndex)
    {
        if (persoButtons == null) return;

        for (int i = 0; i < persoButtons.Length; i++)
        {
            if (persoButtons[i] == null) continue;

            LeanTween.cancel(persoButtons[i].gameObject);

            if (i == selectedIndex)
            {
                LeanTween.scale(persoButtons[i], new Vector3(1.15f, 1.15f, 1f), 0.2f)
                    .setEase(LeanTweenType.easeOutBack);
            }
            else
            {
                LeanTween.scale(persoButtons[i], Vector3.one, 0.15f)
                    .setEase(LeanTweenType.easeOutQuad);
            }
        }
    }

    // -------------------------------------------------------------------------
    // Action Buttons
    // -------------------------------------------------------------------------

    private void AnimateStartButtonIn()
    {
        if (startButton == null) return;

        startButton.localScale = new Vector3(1f, 0f, 1f);

        float delay = staggerDelay * (2 + (persoButtons != null ? persoButtons.Length : 0));

        LeanTween.scaleY(startButton.gameObject, 1f, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(delay);
    }

    /// <summary>
    /// Squeeze bounce on the Start button. Wire to its OnClick event.
    /// </summary>
    public void AnimateStartButtonPress()
    {
        if (startButton == null) return;
        AnimateButtonPress(startButton, Vector3.one);
    }

    private void AnimateRetourButtonIn()
    {
        if (retourButton == null) return;

        retourButton.anchoredPosition = new Vector2(retourOrigin.x + OffscreenLeft, retourOrigin.y);

        LeanTween.move(retourButton, retourOrigin, introDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setDelay(staggerDelay);
    }

    /// <summary>
    /// Squeeze bounce on the Retour button. Wire to its OnClick event.
    /// </summary>
    public void AnimateRetourButtonPress()
    {
        if (retourButton == null) return;
        AnimateButtonPress(retourButton, Vector3.one);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void AnimateButtonPress(RectTransform button, Vector3 baseScale)
    {
        LeanTween.cancel(button.gameObject);

        Vector3 squeezed = baseScale * 0.85f;

        LeanTween.scale(button, squeezed, buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(button, baseScale, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

    private void CacheOrigins()
    {
        if (title != null)        titleOrigin    = title.anchoredPosition;
        if (subtitle != null)     subtitleOrigin = subtitle.anchoredPosition;
        if (retourButton != null) retourOrigin   = retourButton.anchoredPosition;
    }
}

