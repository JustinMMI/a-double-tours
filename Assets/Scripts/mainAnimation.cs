using UnityEngine;

public class mainAnimation : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform header;
    public RectTransform[] persoButtons;
    public RectTransform startButton;

    [Header("Timing")]
    public float introDuration = 0.5f;
    public float staggerDelay = 0.08f;
    public float buttonBounceDuration = 0.12f;

    private const float OffscreenTop = 400f;
    private const float OffscreenBottom = -400f;

    private Vector2 headerOrigin;
    private Vector2[] persoButtonOrigins;
    private Vector2 startButtonOrigin;

    void Start()
    {
        CacheOrigins();
        AnimateIntro();
    }

    public void AnimateIntro()
    {
        AnimateHeaderIn();
        AnimatePersoButtonsIn();
        AnimateStartButtonIn();
    }

    private void AnimateHeaderIn()
    {
        if (header == null) return;

        header.anchoredPosition = new Vector2(headerOrigin.x, headerOrigin.y + OffscreenTop);

        LeanTween.move(header, headerOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(0f);
    }

    private void AnimatePersoButtonsIn()
    {
        if (persoButtons == null) return;

        for (int i = 0; i < persoButtons.Length; i++)
        {
            RectTransform btn = persoButtons[i];
            if (btn == null) continue;

            Vector2 origin = persoButtonOrigins[i];
            btn.anchoredPosition = new Vector2(origin.x, origin.y + OffscreenBottom);

            float delay = staggerDelay * i;

            LeanTween.move(btn, origin, introDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(delay);
        }
    }

    public void AnimatePersoButtonPress(RectTransform button)
    {
        if (button == null) return;

        LeanTween.cancel(button.gameObject);

        LeanTween.scale(button, new Vector3(0.85f, 0.85f, 1f), buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(button, Vector3.one, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

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

    private void AnimateStartButtonIn()
    {
        if (startButton == null) return;

        startButton.anchoredPosition = new Vector2(startButtonOrigin.x, startButtonOrigin.y + OffscreenBottom);

        float delay = staggerDelay * (persoButtons != null ? persoButtons.Length : 0);

        LeanTween.move(startButton, startButtonOrigin, introDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(delay);
    }

    public void AnimateStartButtonPress()
    {
        if (startButton == null) return;

        LeanTween.cancel(startButton.gameObject);

        LeanTween.scale(startButton, new Vector3(0.88f, 0.88f, 1f), buttonBounceDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(startButton, Vector3.one, buttonBounceDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

    private void CacheOrigins()
    {
        if (header != null)
            headerOrigin = header.anchoredPosition;

        if (persoButtons != null)
        {
            persoButtonOrigins = new Vector2[persoButtons.Length];
            for (int i = 0; i < persoButtons.Length; i++)
            {
                if (persoButtons[i] != null)
                    persoButtonOrigins[i] = persoButtons[i].anchoredPosition;
            }
        }

        if (startButton != null)
            startButtonOrigin = startButton.anchoredPosition;
    }
}

