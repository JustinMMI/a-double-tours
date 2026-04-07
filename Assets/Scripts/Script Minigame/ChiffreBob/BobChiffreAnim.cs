using UnityEngine;

/// <summary>
/// Handles all LeanTween animations for the Chiffre De Bob minigame.
/// Attach to the BobChiffreAnim GameObject and wire up the references in the Inspector.
/// Call the public methods from GameManager at the appropriate moments.
/// </summary>
public class BobChiffreAnim : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform startPanel;
    public RectTransform gamePanel;
    public RectTransform resultPanel;

    [Header("Game Panel Elements")]
    public RectTransform playerTurnText;
    public RectTransform sliderValueText;
    public RectTransform slider;
    public RectTransform submitButton;

    [Header("Result Panel Elements")]
    public RectTransform resultText;

    [Header("Timing")]
    public float panelSlideDuration = 0.45f;
    public float punchDuration = 0.35f;
    public float buttonBounceDuration = 0.15f;

    private const float OffscreenRight = 1200f;
    private const float OffscreenLeft = -1200f;
    private const float OffscreenBottom = -900f;

    // -------------------------------------------------------------------------
    // Start Panel
    // -------------------------------------------------------------------------

    /// <summary>
    /// Slides the start panel in from the bottom on game launch.
    /// </summary>
    public void AnimateStartPanelIn()
    {
        if (startPanel == null) return;

        startPanel.gameObject.SetActive(true);

        Vector2 origin = startPanel.anchoredPosition;
        startPanel.anchoredPosition = new Vector2(origin.x, OffscreenBottom);

        LeanTween.move(startPanel, new Vector2(origin.x, 0f), panelSlideDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Slides the start panel out to the left before the game begins.
    /// </summary>
    public void AnimateStartPanelOut()
    {
        if (startPanel == null) return;

        LeanTween.move(startPanel, new Vector2(OffscreenLeft, startPanel.anchoredPosition.y), panelSlideDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => startPanel.gameObject.SetActive(false));
    }

    // -------------------------------------------------------------------------
    // Game Panel
    // -------------------------------------------------------------------------

    /// <summary>
    /// Slides the game panel in from the right. Call when starting a new player turn.
    /// </summary>
    public void AnimateGamePanelIn()
    {
        if (gamePanel == null) return;

        gamePanel.gameObject.SetActive(true);
        gamePanel.anchoredPosition = new Vector2(OffscreenRight, gamePanel.anchoredPosition.y);

        LeanTween.move(gamePanel, new Vector2(0f, gamePanel.anchoredPosition.y), panelSlideDuration)
            .setEase(LeanTweenType.easeOutCubic);
    }

    /// <summary>
    /// Slides the game panel out to the left. Call after the last player guesses.
    /// </summary>
    public void AnimateGamePanelOut()
    {
        if (gamePanel == null) return;

        LeanTween.move(gamePanel, new Vector2(OffscreenLeft, gamePanel.anchoredPosition.y), panelSlideDuration)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(() => gamePanel.gameObject.SetActive(false));
    }

    /// <summary>
    /// Punch-scales the player name text to draw attention on each new turn.
    /// </summary>
    public void AnimatePlayerTurnPunch()
    {
        if (playerTurnText == null) return;

        LeanTween.cancel(playerTurnText.gameObject);
        playerTurnText.localScale = Vector3.one;

        LeanTween.scale(playerTurnText, new Vector3(1.3f, 1.3f, 1f), punchDuration * 0.4f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(playerTurnText, Vector3.one, punchDuration * 0.6f)
                    .setEase(LeanTweenType.easeOutElastic);
            });
    }

    /// <summary>
    /// Quick squeeze on the submit button when pressed.
    /// </summary>
    public void AnimateSubmitButtonBounce()
    {
        if (submitButton == null) return;

        LeanTween.cancel(submitButton.gameObject);
        submitButton.localScale = Vector3.one;

        LeanTween.scale(submitButton, new Vector3(0.88f, 0.88f, 1f), buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(submitButton, Vector3.one, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

    // -------------------------------------------------------------------------
    // Result Panel
    // -------------------------------------------------------------------------

    /// <summary>
    /// Pops the result panel in with a scale animation, then bounces the result text.
    /// </summary>
    public void AnimateResultPanelIn()
    {
        if (resultPanel == null) return;

        resultPanel.gameObject.SetActive(true);
        resultPanel.localScale = Vector3.zero;

        LeanTween.scale(resultPanel, Vector3.one, panelSlideDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(AnimateResultTextBounce);
    }

    /// <summary>
    /// Shrinks the result panel out. Call at the start of RestartGame.
    /// </summary>
    public void AnimateResultPanelOut()
    {
        if (resultPanel == null) return;

        LeanTween.scale(resultPanel, Vector3.zero, panelSlideDuration * 0.8f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => resultPanel.gameObject.SetActive(false));
    }

    private void AnimateResultTextBounce()
    {
        if (resultText == null) return;

        resultText.localScale = new Vector3(0.5f, 0.5f, 1f);

        LeanTween.scale(resultText, Vector3.one, punchDuration)
            .setEase(LeanTweenType.easeOutBack);
    }
}

