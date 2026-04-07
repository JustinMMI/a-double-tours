using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CacheCacheUI : MonoBehaviour
{

    [Header("Instruction Panel")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Bush Buttons (exactly 5)")]
    [SerializeField] private Button[] bushButtons;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Button returnButton;

    [Header("Navigation")]
    [SerializeField] private CacheCacheManager manager;


    private static readonly Color DefaultBushColor  = Color.white;
    private static readonly Color SelectedBushColor = new Color(0.4f, 0.8f, 0.4f);
    private static readonly Color CaughtBushColor   = new Color(1f,   0.3f, 0.3f);
    private static readonly Color SafeBushColor     = new Color(0.3f, 0.7f, 1f);
    private static readonly Color DisabledBushColor = new Color(0.55f, 0.55f, 0.55f);

    private Coroutine countdownCoroutine;


    private void Awake()
    {
        resultPanel.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);

        returnButton.onClick.AddListener(manager.ReturnToGame);

        for (int i = 0; i < bushButtons.Length; i++)
        {
            int capturedIndex = i;
            bushButtons[i].onClick.AddListener(() => OnBushClicked(capturedIndex));
        }
    }


    public void ShowHiderSelection(string hiderName, int hiderIndex, int totalHiders, HashSet<int> disabledBushes)
    {
        StopCountdownIfRunning();
        resultPanel.SetActive(false);

        ResetBushColors(disabledBushes);
        ApplyDisabledBushes(disabledBushes);

        instructionText.text =
            $"Cacheur {hiderIndex + 1}/{totalHiders}\n" +
            $"<b>{hiderName}</b>, choisis ton buisson !";
    }

    public void ShowSeekerSelection(string seekerName, HashSet<int> disabledBushes, int attemptsLeft)
    {
        StopCountdownIfRunning();
        resultPanel.SetActive(false);

        ResetBushColors(disabledBushes);
        ApplyDisabledBushes(disabledBushes);

        string attemptsWarning = attemptsLeft == 1
            ? "\n<color=#FF6666> Dernier essai avant défaite !</color>"
            : "";

        instructionText.text =
            $"C'est au tour du chasseur !\n" +
            $"<b>{seekerName}</b>, dans quel buisson cherches-tu ?" +
            attemptsWarning;
    }

    public void ShowResult(
        string seekerName,
        int seekerBushIndex,
        List<string> caught,
        List<string> escaped,
        Dictionary<string, int> hiderChoices,
        int bushCount,
        HashSet<int> disabledBushes,
        bool seekerWins,
        bool seekerLoses,
        bool canContinue)
    {
        SetBushButtonsInteractable(false);

        for (int i = 0; i < bushCount; i++)
        {
            if (disabledBushes.Contains(i) && i != seekerBushIndex)
            {
                ColorBush(i, DisabledBushColor);
                continue;
            }
            if (i == seekerBushIndex)
            {
                ColorBush(i, caught.Count > 0 ? CaughtBushColor : SafeBushColor);
                continue;
            }

            bool hasHider = false;
            foreach (var kvp in hiderChoices)
                if (kvp.Value == i) { hasHider = true; break; }

            ColorBush(i, hasHider ? SelectedBushColor : DefaultBushColor);
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (caught.Count > 0)
            sb.AppendLine($" <b>{string.Join(", ", caught)}</b> à été éliminé(s)");
        else
            sb.AppendLine("Il n'y a personne dans ce buisson !");

        if (escaped.Count > 1)
            sb.AppendLine($"<b>{string.Join(", ", escaped)}</b> sont encore cachés");
        else
            sb.AppendLine($"<b>{string.Join(", ", escaped)}</b> est encore caché");

        if (seekerWins)
            sb.AppendLine("\n<b>Le chasseur a trouvé tout le monde !</b>");

        else if (seekerLoses)
            sb.AppendLine("\n<b>Les cacheurs ont gagné !</b>");
            
        else if (!canContinue)
            sb.AppendLine("\n<b>Les cacheurs restants ont gagné !</b>");



        resultText.text = sb.ToString();
        resultPanel.SetActive(true);

        if (canContinue && countdownText != null)
        {
            StopCountdownIfRunning();
            countdownCoroutine = StartCoroutine(ShowCountdown(5));
        }
        else if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }


    private IEnumerator ShowCountdown(int seconds)
    {
        countdownText.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(false);

        for (int i = seconds; i >= 1; i--)
        {
            countdownText.text = $"Reprise dans {i}...";
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);

        countdownCoroutine = null;
    }

    private void StopCountdownIfRunning()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private void OnBushClicked(int bushIndex)
    {
        if (manager.CurrentPhase == CacheCacheManager.GamePhase.HiderSelection)
        {
            ColorBush(bushIndex, SelectedBushColor);
            SetBushButtonsInteractable(false);
            manager.OnHiderChoseBush(bushIndex);
        }
        else if (manager.CurrentPhase == CacheCacheManager.GamePhase.SeekerSelection)
        {
            SetBushButtonsInteractable(false);
            manager.OnSeekerChoseBush(bushIndex);
        }
    }

    private void ResetBushColors(HashSet<int> disabledBushes)
    {
        for (int i = 0; i < bushButtons.Length; i++)
            bushButtons[i].GetComponent<Image>().color =
                disabledBushes.Contains(i) ? DisabledBushColor : DefaultBushColor;
    }

    private void ApplyDisabledBushes(HashSet<int> disabledBushes)
    {
        for (int i = 0; i < bushButtons.Length; i++)
        {
            bool disabled = disabledBushes.Contains(i);
            bushButtons[i].interactable = !disabled;
            if (disabled)
                bushButtons[i].GetComponent<Image>().color = DisabledBushColor;
        }
    }

    private void ColorBush(int index, Color color)
    {
        if (index >= 0 && index < bushButtons.Length)
            bushButtons[index].GetComponent<Image>().color = color;
    }

    private void SetBushButtonsInteractable(bool interactable)
    {
        foreach (Button btn in bushButtons)
            btn.interactable = interactable;
    }
}