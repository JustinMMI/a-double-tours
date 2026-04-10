using UnityEngine;

public class gonflementBallonAnim : MonoBehaviour
{
    [Header("Pump Buttons")]
    public RectTransform buttonJoueur1;   // Canvas/Button   — ancré bas-gauche
    public RectTransform buttonJoueur2;   // Canvas/Button (1) — ancré bas-droite

    [Header("Validate Button")]
    public RectTransform buttonValide;    // Canvas/Button valide — scale 1.5

    [Header("Player Labels")]
    public RectTransform joueur1Text;     // Canvas/Joueur 1
    public RectTransform joueur2Text;     // Canvas/Joueur 2

    [Header("HUD")]
    public RectTransform timer;           // Canvas/Timer

    [Header("Victory")]
    public RectTransform victoryText;     // Canvas/Victory Text

    [Header("Explanation Panel")]
    public RectTransform panel;           // Canvas/Panel — full-stretch
    public RectTransform quitterButton;   // Canvas/Panel/Quitter Panel — scale 1.5

    [Header("Balloons")]
    public RectTransform ballon1;         // Canvas/Ballon
    public RectTransform ballon2;         // Canvas/Ballon2

    [Header("Timing")]
    public float introDuration = 0.4f;
    public float staggerDelay = 0.07f;
    public float buttonBounceDuration = 0.12f;

    // Base scales cached at Start.
    private Vector3 button1BaseScale;
    private Vector3 button2BaseScale;
    private Vector3 buttonValideBaseScale;
    private Vector3 quitterBaseScale;
    private Vector3 ballon1BaseScale;
    private Vector3 ballon2BaseScale;

    // anchoredPosition origins for move()-based elements.
    private Vector2 button1Origin;
    private Vector2 button2Origin;
    private Vector2 joueur1Origin;
    private Vector2 joueur2Origin;
    private Vector2 timerOrigin;

    private const float OffscreenBottom = -300f;
    private const float OffscreenLeft   = -600f;
    private const float OffscreenRight  =  600f;
    private const float OffscreenTop    =  300f;

    void Start()
    {
        CacheOriginsAndScales();
        AnimateIntro();
    }

    public void AnimateIntro()
    {
        AnimateTimerIn();
        AnimateJoueurLabelsIn();
        AnimatePumpButtonsIn();
        AnimateButtonValideIn();
        AnimateBalloonsIn();
    }

    private void AnimateTimerIn()
    {
        if (timer == null) return;

        timer.anchoredPosition = new Vector2(timerOrigin.x, timerOrigin.y + OffscreenTop);

        LeanTween.move(timer, timerOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    public void AnimateTimerPulse()
    {
        if (timer == null) return;

        LeanTween.cancel(timer.gameObject);
        timer.localScale = Vector3.one;

        LeanTween.scale(timer, new Vector3(1.25f, 1.25f, 1f), 0.15f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(timer, Vector3.one, 0.2f)
                    .setEase(LeanTweenType.easeOutElastic);
            });
    }

    private void AnimateJoueurLabelsIn()
    {
        if (joueur1Text != null)
        {
            joueur1Text.anchoredPosition = new Vector2(joueur1Origin.x + OffscreenLeft, joueur1Origin.y);
            LeanTween.move(joueur1Text, joueur1Origin, introDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(staggerDelay);
        }

        if (joueur2Text != null)
        {
            joueur2Text.anchoredPosition = new Vector2(joueur2Origin.x + OffscreenRight, joueur2Origin.y);
            LeanTween.move(joueur2Text, joueur2Origin, introDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(staggerDelay);
        }
    }

    private void AnimatePumpButtonsIn()
    {
        if (buttonJoueur1 != null)
        {
            buttonJoueur1.anchoredPosition = new Vector2(button1Origin.x, button1Origin.y + OffscreenBottom);
            LeanTween.move(buttonJoueur1, button1Origin, introDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(staggerDelay * 2);
        }

        if (buttonJoueur2 != null)
        {
            buttonJoueur2.anchoredPosition = new Vector2(button2Origin.x, button2Origin.y + OffscreenBottom);
            LeanTween.move(buttonJoueur2, button2Origin, introDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(staggerDelay * 2);
        }
    }

    public void AnimateButton1Press()
    {
        if (buttonJoueur1 == null) return;
        AnimateButtonPress(buttonJoueur1, button1BaseScale);
    }

    public void AnimateButton2Press()
    {
        if (buttonJoueur2 == null) return;
        AnimateButtonPress(buttonJoueur2, button2BaseScale);
    }

    private void AnimateButtonValideIn()
    {
        if (buttonValide == null) return;

        Vector3 target = buttonValideBaseScale;
        buttonValide.localScale = new Vector3(target.x, 0f, target.z);

        LeanTween.scaleY(buttonValide.gameObject, target.y, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(staggerDelay * 3);
    }

    public void AnimateButtonValidePress()
    {
        if (buttonValide == null) return;
        AnimateButtonPress(buttonValide, buttonValideBaseScale);
    }

    private void AnimateBalloonsIn()
    {
        AnimateSingleBalloonIn(ballon1, ballon1BaseScale, staggerDelay * 2);
        AnimateSingleBalloonIn(ballon2, ballon2BaseScale, staggerDelay * 3);
    }

    private void AnimateSingleBalloonIn(RectTransform ballon, Vector3 targetScale, float delay)
    {
        if (ballon == null) return;

        ballon.localScale = Vector3.zero;

        LeanTween.scale(ballon, targetScale, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(delay);
    }

    public void AnimateBallon1Pop()
    {
        AnimateBalloonPop(ballon1);
    }

    public void AnimateBallon2Pop()
    {
        AnimateBalloonPop(ballon2);
    }

    private void AnimateBalloonPop(RectTransform ballon)
    {
        if (ballon == null) return;

        LeanTween.cancel(ballon.gameObject);

        Vector3 current = ballon.localScale;

        LeanTween.scale(ballon, current * 1.4f, 0.1f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(ballon, Vector3.zero, 0.15f)
                    .setEase(LeanTweenType.easeInBack);
            });
    }

    public void AnimatePanelOut()
    {
        if (panel == null) return;

        LeanTween.scale(panel, Vector3.zero, introDuration * 0.8f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                panel.localScale = Vector3.one;
                panel.gameObject.SetActive(false);
            });
    }

    public void AnimateQuitterButtonPress()
    {
        if (quitterButton == null) return;
        AnimateButtonPress(quitterButton, quitterBaseScale);
    }

    public void AnimateVictoryIn()
    {
        if (victoryText == null) return;

        victoryText.localScale = Vector3.zero;

        LeanTween.scale(victoryText, Vector3.one, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                LeanTween.scale(victoryText, new Vector3(1.1f, 1.1f, 1f), 0.4f)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong();
            });
    }

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
        if (buttonJoueur1 != null) { button1BaseScale = buttonJoueur1.localScale; button1Origin = buttonJoueur1.anchoredPosition; }
        if (buttonJoueur2 != null) { button2BaseScale = buttonJoueur2.localScale; button2Origin = buttonJoueur2.anchoredPosition; }
        if (buttonValide != null)  buttonValideBaseScale = buttonValide.localScale;
        if (quitterButton != null) quitterBaseScale      = quitterButton.localScale;
        if (ballon1 != null)       ballon1BaseScale      = ballon1.localScale;
        if (ballon2 != null)       ballon2BaseScale      = ballon2.localScale;
        if (joueur1Text != null)   joueur1Origin = joueur1Text.anchoredPosition;
        if (joueur2Text != null)   joueur2Origin = joueur2Text.anchoredPosition;
        if (timer != null)         timerOrigin   = timer.anchoredPosition;
    }
}

