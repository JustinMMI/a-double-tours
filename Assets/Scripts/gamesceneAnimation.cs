using UnityEngine;

/// <summary>
/// Handles all LeanTween animations for the GameScene.
/// Attach to the gamesceneAnim GameObject and wire up references in the Inspector.
/// AnimateIntro() is called automatically on Start.
/// Call the public methods from your game logic or Unity Button OnClick events.
/// </summary>
public class gamesceneAnimation : MonoBehaviour
{
    [Header("Bob Group")]
    public RectTransform bob;         // Parent of Image + DuelButton
    public RectTransform bobImage;    // Canvas/Bob/Image  (has Button)
    public RectTransform duelButton;  // Canvas/Bob/DuelButton

    [Header("UI Elements")]
    public RectTransform bobText;
    public RectTransform nextButton;
    public RectTransform quitButton;
    public RectTransform okButton;
    public RectTransform rerollButton;

    [Header("Timing")]
    public float introDuration = 0.45f;
    public float staggerDelay = 0.07f;
    public float buttonBounceDuration = 0.12f;

    // Base scales cached at Start — handles elements whose localScale != (1,1,1).
    private Vector3 bobImageBaseScale;
    private Vector3 duelButtonBaseScale;
    private Vector3 okButtonBaseScale;
    private Vector3 rerollButtonBaseScale;
    private Vector3 nextButtonBaseScale;
    private Vector3 quitButtonBaseScale;

    // anchoredPosition origins for elements that use move().
    private Vector2 bobTextOrigin;
    private Vector2 nextButtonOrigin;
    private Vector2 quitButtonOrigin;
    private Vector2 okButtonOrigin;
    private Vector2 rerollButtonOrigin;

    private const float OffscreenRight = 600f;
    private const float OffscreenBottom = -300f;
    private const float OffscreenTop = 300f;

    void Start()
    {
        CacheOriginsAndScales();
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
        AnimateBobIn();
        AnimateBobTextIn();
        AnimateNextButtonIn();
        AnimateQuitButtonIn();
        AnimateOkButtonIn();
        AnimateRerollButtonIn();
    }

    // -------------------------------------------------------------------------
    // Bob group  (parent pop → children settle)
    // -------------------------------------------------------------------------

    private void AnimateBobIn()
    {
        if (bob == null) return;

        bob.localScale = Vector3.zero;

        LeanTween.scale(bob, Vector3.one, introDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Squeeze + elastic pop on Bob's image when clicked.
    /// Wire to Canvas/Bob/Image Button OnClick.
    /// </summary>
    public void AnimateBobImageClick()
    {
        if (bobImage == null) return;

        LeanTween.cancel(bobImage.gameObject);

        Vector3 squeezed = bobImageBaseScale * 0.85f;
        Vector3 overshot = bobImageBaseScale * 1.1f;

        LeanTween.scale(bobImage, squeezed, buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(bobImage, overshot, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(bobImage, bobImageBaseScale, 0.2f)
                            .setEase(LeanTweenType.easeOutElastic);
                    });
            });
    }

    // -------------------------------------------------------------------------
    // BobText
    // -------------------------------------------------------------------------

    private void AnimateBobTextIn()
    {
        if (bobText == null) return;

        bobText.anchoredPosition = new Vector2(bobTextOrigin.x, bobTextOrigin.y + OffscreenBottom);

        LeanTween.move(bobText, bobTextOrigin, introDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setDelay(staggerDelay);
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
    // NextButton
    // -------------------------------------------------------------------------

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
        AnimateButtonPress(nextButton, nextButtonBaseScale);
    }

    // -------------------------------------------------------------------------
    // Quit button
    // -------------------------------------------------------------------------

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
        AnimateButtonPress(quitButton, quitButtonBaseScale);
    }

    // -------------------------------------------------------------------------
    // okButton
    // -------------------------------------------------------------------------

    private void AnimateOkButtonIn()
    {
        if (okButton == null) return;

        okButton.localScale = Vector3.zero;

        LeanTween.scale(okButton, okButtonBaseScale, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(staggerDelay * 3);
    }

    /// <summary>
    /// Squeeze bounce on the OK button. Wire to its OnClick event.
    /// </summary>
    public void AnimateOkButtonPress()
    {
        if (okButton == null) return;
        AnimateButtonPress(okButton, okButtonBaseScale);
    }

    // -------------------------------------------------------------------------
    // rerollButton
    // -------------------------------------------------------------------------

    private void AnimateRerollButtonIn()
    {
        if (rerollButton == null) return;

        rerollButton.localScale = Vector3.zero;

        LeanTween.scale(rerollButton, rerollButtonBaseScale, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(staggerDelay * 4);
    }

    /// <summary>
    /// Squeeze bounce on the Reroll button. Wire to its OnClick event.
    /// </summary>
    public void AnimateRerollButtonPress()
    {
        if (rerollButton == null) return;
        AnimateButtonPress(rerollButton, rerollButtonBaseScale);
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

    private void CacheOriginsAndScales()
    {
        if (bobImage != null)    bobImageBaseScale    = bobImage.localScale;
        if (duelButton != null)  duelButtonBaseScale  = duelButton.localScale;
        if (okButton != null)    okButtonBaseScale    = okButton.localScale;
        if (rerollButton != null) rerollButtonBaseScale = rerollButton.localScale;
        if (nextButton != null)  nextButtonBaseScale  = nextButton.localScale;
        if (quitButton != null)  quitButtonBaseScale  = quitButton.localScale;

        if (bobText != null)     bobTextOrigin     = bobText.anchoredPosition;
        if (nextButton != null)  nextButtonOrigin  = nextButton.anchoredPosition;
        if (quitButton != null)  quitButtonOrigin  = quitButton.anchoredPosition;
        if (okButton != null)    okButtonOrigin    = okButton.anchoredPosition;
        if (rerollButton != null) rerollButtonOrigin = rerollButton.anchoredPosition;
    }
}

