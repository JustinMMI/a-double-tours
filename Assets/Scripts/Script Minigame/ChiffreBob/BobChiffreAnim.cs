using UnityEngine;

/// <summary>
/// Handles all LeanTween animations for the Chiffre De Bob minigame.
/// Attach to BobChiffreAnim and wire references in the Inspector.
/// Call the public methods from GameManager at the appropriate moments.
/// </summary>
public class BobChiffreAnim : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform startPanel;
    public RectTransform gamePanel;
    public RectTransform resultPanel;

    [Header("Start Panel Elements")]
    public RectTransform startButton;
    public RectTransform rulesText;

    [Header("Game Panel Elements")]
    public RectTransform playerTurnText;
    public RectTransform sliderValueText;
    public RectTransform slider;
    public RectTransform submitButton;

    [Header("Result Panel Elements")]
    public RectTransform resultText;
    public RectTransform resultButton;

    [Header("Timing")]
    public float panelDuration = 0.4f;
    public float punchDuration = 0.35f;
    public float buttonBounceDuration = 0.15f;

    // Base scales cached at Start to handle buttons whose localScale != (1,1,1).
    private Vector3 startButtonBaseScale;
    private Vector3 submitButtonBaseScale;
    private Vector3 resultButtonBaseScale;

    void Start()
    {
        if (startButton != null)  startButtonBaseScale  = startButton.localScale;
        if (submitButton != null) submitButtonBaseScale = submitButton.localScale;
        if (resultButton != null) resultButtonBaseScale = resultButton.localScale;
    }

    // -------------------------------------------------------------------------
    // Start Panel
    // -------------------------------------------------------------------------

    /// <summary>
    /// Reveals the start panel by sliding its children up from below.
    /// </summary>
    public void AnimateStartPanelIn()
    {
        if (startPanel == null) return;

        startPanel.gameObject.SetActive(true);
        startPanel.localScale = Vector3.one;

        if (rulesText != null)
        {
            rulesText.localScale = new Vector3(1f, 0f, 1f);
            LeanTween.scaleY(rulesText.gameObject, 1f, panelDuration)
                .setEase(LeanTweenType.easeOutBack);
        }

        if (startButton != null)
        {
            Vector3 target = startButtonBaseScale;
            startButton.localScale = new Vector3(target.x, 0f, target.z);
            LeanTween.scaleY(startButton.gameObject, target.y, panelDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(0.07f);
        }
    }

    /// <summary>
    /// Collapses the start panel before the game begins.
    /// </summary>
    public void AnimateStartPanelOut()
    {
        if (startPanel == null) return;

        LeanTween.scale(startPanel, new Vector3(1f, 0f, 1f), panelDuration * 0.8f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                startPanel.localScale = Vector3.one;
                startPanel.gameObject.SetActive(false);
            });
    }

    // -------------------------------------------------------------------------
    // Game Panel
    // -------------------------------------------------------------------------

    /// <summary>
    /// Pops the game panel in. Call when starting a new player turn.
    /// </summary>
    public void AnimateGamePanelIn()
    {
        if (gamePanel == null) return;

        gamePanel.gameObject.SetActive(true);
        gamePanel.localScale = new Vector3(0.85f, 0.85f, 1f);

        LeanTween.scale(gamePanel, Vector3.one, panelDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Collapses the game panel. Call after the last player guesses.
    /// </summary>
    public void AnimateGamePanelOut()
    {
        if (gamePanel == null) return;

        LeanTween.scale(gamePanel, new Vector3(0.85f, 0.85f, 1f), panelDuration * 0.8f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                gamePanel.localScale = Vector3.one;
                gamePanel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// Punch-scales the player name text on each new turn.
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
    /// Squeeze bounce on the submit button when pressed.
    /// Respects the button's base localScale (which may differ from Vector3.one).
    /// </summary>
    public void AnimateSubmitButtonBounce()
    {
        if (submitButton == null) return;

        LeanTween.cancel(submitButton.gameObject);

        Vector3 squeezed = submitButtonBaseScale * 0.85f;

        LeanTween.scale(submitButton, squeezed, buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(submitButton, submitButtonBaseScale, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

    // -------------------------------------------------------------------------
    // Result Panel
    // -------------------------------------------------------------------------

    /// <summary>
    /// Pops the result panel in then bounces the result text and button.
    /// </summary>
    public void AnimateResultPanelIn()
    {
        if (resultPanel == null) return;

        resultPanel.gameObject.SetActive(true);
        resultPanel.localScale = Vector3.zero;

        LeanTween.scale(resultPanel, Vector3.one, panelDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(AnimateResultChildren);
    }

    /// <summary>
    /// Collapses the result panel. Call at the start of RestartGame.
    /// </summary>
    public void AnimateResultPanelOut()
    {
        if (resultPanel == null) return;

        LeanTween.scale(resultPanel, Vector3.zero, panelDuration * 0.8f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                resultPanel.localScale = Vector3.one;
                resultPanel.gameObject.SetActive(false);
            });
    }

    private void AnimateResultChildren()
    {
        if (resultText != null)
        {
            resultText.localScale = new Vector3(0.5f, 0.5f, 1f);
            LeanTween.scale(resultText, Vector3.one, punchDuration)
                .setEase(LeanTweenType.easeOutElastic);
        }

        if (resultButton != null)
        {
            Vector3 target = resultButtonBaseScale;
            resultButton.localScale = new Vector3(target.x, 0f, target.z);
            LeanTween.scaleY(resultButton.gameObject, target.y, panelDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(0.1f);
        }
    }
}

