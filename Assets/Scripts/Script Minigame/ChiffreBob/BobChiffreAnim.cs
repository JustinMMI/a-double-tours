using UnityEngine;

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


    public void AnimateStartPanelIn()
    {
        if (startPanel == null) return;

        startPanel.gameObject.SetActive(true);

        Vector2 origin = startPanel.anchoredPosition;
        startPanel.anchoredPosition = new Vector2(origin.x, OffscreenBottom);

        LeanTween.move(startPanel, new Vector2(origin.x, 0f), panelSlideDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    public void AnimateStartPanelOut()
    {
        if (startPanel == null) return;

        LeanTween.move(startPanel, new Vector2(OffscreenLeft, startPanel.anchoredPosition.y), panelSlideDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => startPanel.gameObject.SetActive(false));
    }

    public void AnimateGamePanelIn()
    {
        if (gamePanel == null) return;

        gamePanel.gameObject.SetActive(true);
        gamePanel.anchoredPosition = new Vector2(OffscreenRight, gamePanel.anchoredPosition.y);

        LeanTween.move(gamePanel, new Vector2(0f, gamePanel.anchoredPosition.y), panelSlideDuration)
            .setEase(LeanTweenType.easeOutCubic);
    }


    public void AnimateGamePanelOut()
    {
        if (gamePanel == null) return;

        LeanTween.move(gamePanel, new Vector2(OffscreenLeft, gamePanel.anchoredPosition.y), panelSlideDuration)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(() => gamePanel.gameObject.SetActive(false));
    }

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

    public void AnimateResultPanelIn()
    {
        if (resultPanel == null) return;

        resultPanel.gameObject.SetActive(true);
        resultPanel.localScale = Vector3.zero;

        LeanTween.scale(resultPanel, Vector3.one, panelSlideDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(AnimateResultTextBounce);
    }

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

