using UnityEngine;

/// <summary>
/// Handles all LeanTween animations for the GameScene.
/// Attach to the gamesceneAnim GameObject and wire up references in the Inspector.
/// AnimateIntro() is called automatically on Start.
/// Call the public methods from your game logic or Unity Button OnClick events.
/// </summary>
public class gamesceneAnimation : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform bobImage;
    public RectTransform bobText;
    public RectTransform nextButton;
    public RectTransform duelButton;
    public RectTransform quitButton;

    [Header("Timing")]
    public float introDuration = 0.45f;
    public float staggerDelay = 0.07f;
    public float buttonBounceDuration = 0.12f;

    private const float OffscreenRight = 600f;
    private const float OffscreenLeft = -600f;
    private const float OffscreenTop = 300f;
    private const float OffscreenBottom = -300f;

    private Vector2 bobImageOrigin;
    private Vector2 bobTextOrigin;
    private Vector2 nextButtonOrigin;
    private Vector2 duelButtonOrigin;
    private Vector2 quitButtonOrigin;

    void Start()
    {
        CacheOrigins();
        AnimateIntro();
    }

    // -------------------------------------------------------------------------
    // Intro
    // -------------------------------------------------------------------------

    /// <summary>
    /// Plays the full entrance animation for all GameScene UI elements.
    /// </summary>
    public void AnimateIntro()
    {
        AnimateBobImageIn();
        AnimateBobTextIn();
        AnimateNextButtonIn();
        AnimateDuelButtonIn();
        AnimateQuitButtonIn();
    }

    // -------------------------------------------------------------------------
    // Bob Image
    // -------------------------------------------------------------------------

    private void AnimateBobImageIn()
    {
        if (bobImage == null) return;

        bobImage.localScale = Vector3.zero;

        LeanTween.scale(bobImage, Vector3.one, introDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Squeeze + bounce on Bob's image when clicked.
    /// Wire this to the Image Button's OnClick event.
    /// </summary>
    public void AnimateBobImageClick()
    {
        if (bobImage == null) return;

        LeanTween.cancel(bobImage.gameObject);

        // Squish down
        LeanTween.scale(bobImage, new Vector3(0.88f, 0.88f, 1f), buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                // Pop back with overshoot
                LeanTween.scale(bobImage, new Vector3(1.12f, 1.12f, 1f), buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(bobImage, Vector3.one, 0.2f)
                            .setEase(LeanTweenType.easeOutElastic);
                    });
            });
    }

    /// <summary>
    /// Subtle pop on BobText each time its content is updated.
    /// </summary>
    public void AnimateBobTextUpdate()
    {
        if (bobText == null) return;

        LeanTween.cancel(bobText.gameObject);
        bobText.localScale = new Vector3(0.85f, 0.85f, 1f);

        LeanTween.scale(bobText, Vector3.one, 0.3f)
            .setEase(LeanTweenType.easeOutBack);
    }

    // -------------------------------------------------------------------------
    // Buttons
    // -------------------------------------------------------------------------

    private void AnimateBobTextIn()
    {
        if (bobText == null) return;

        bobText.anchoredPosition = new Vector2(bobTextOrigin.x, bobTextOrigin.y + OffscreenBottom);

        LeanTween.move(bobText, bobTextOrigin, introDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setDelay(staggerDelay);
    }

    private void AnimateNextButtonIn()
    {
        if (nextButton == null) return;

        nextButton.anchoredPosition = new Vector2(nextButtonOrigin.x + OffscreenRight, nextButtonOrigin.y);

        LeanTween.move(nextButton, nextButtonOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(staggerDelay * 2);
    }

    /// <summary>
    /// Squeeze bounce on the Next button. Wire to its OnClick event.
    /// </summary>
    public void AnimateNextButtonPress()
    {
        if (nextButton == null) return;
        AnimateButtonPress(nextButton);
    }

    private void AnimateDuelButtonIn()
    {
        if (duelButton == null) return;

        duelButton.anchoredPosition = new Vector2(duelButtonOrigin.x + OffscreenLeft, duelButtonOrigin.y);

        LeanTween.move(duelButton, duelButtonOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(staggerDelay * 3);
    }

    /// <summary>
    /// Squeeze bounce on the Duel button. Wire to its OnClick event.
    /// </summary>
    public void AnimateDuelButtonPress()
    {
        if (duelButton == null) return;
        AnimateButtonPress(duelButton);
    }

    private void AnimateQuitButtonIn()
    {
        if (quitButton == null) return;

        quitButton.anchoredPosition = new Vector2(quitButtonOrigin.x, quitButtonOrigin.y + OffscreenTop);

        LeanTween.move(quitButton, quitButtonOrigin, introDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setDelay(staggerDelay * 2);
    }

    /// <summary>
    /// Squeeze bounce on the Quit button. Wire to its OnClick event.
    /// </summary>
    public void AnimateQuitButtonPress()
    {
        if (quitButton == null) return;
        AnimateButtonPress(quitButton);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void AnimateButtonPress(RectTransform button)
    {
        LeanTween.cancel(button.gameObject);

        LeanTween.scale(button, new Vector3(0.88f, 0.88f, 1f), buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(button, Vector3.one, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

    private void CacheOrigins()
    {
        if (bobImage != null) bobImageOrigin = bobImage.anchoredPosition;
        if (bobText != null) bobTextOrigin = bobText.anchoredPosition;
        if (nextButton != null) nextButtonOrigin = nextButton.anchoredPosition;
        if (duelButton != null) duelButtonOrigin = duelButton.anchoredPosition;
        if (quitButton != null) quitButtonOrigin = quitButton.anchoredPosition;
    }
}

