using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static readonly string[] PlayerNames = { "Épée", "Jetpack", "Ordinateur", "Casque" };
    private const int TotalPlayerCount = 4;
    private const int SliderDefaultValue = 50;

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject resultPanel;

    public TMP_Text playerTurnText;
    public Slider numberSlider;
    public TMP_Text sliderValueText;
    public TMP_Text resultText;

    public BobChiffreAnim anim;

    private int targetNumber;
    private int currentTurnIndex = 0;

    // Indices of the players competing in the current round (may be a subset on tiebreaker rounds).
    private List<int> activePlayers = new List<int>();

    // Shuffled turn order built from activePlayers each round.
    private List<int> playerOrder = new List<int>();

    // Guesses keyed by player index (0-3).
    private Dictionary<int, int> guesses = new Dictionary<int, int>();

    // Players who tied last round; populated only when a tiebreaker is needed.
    private List<int> tiedPlayers = new List<int>();

    void Start()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        resultPanel.SetActive(false);

        anim?.AnimateStartPanelIn();
    }

    /// <summary>
    /// Called by the Start Game button. Resets to a full 4-player game.
    /// </summary>
    public void StartGame()
    {
        activePlayers.Clear();
        for (int i = 0; i < TotalPlayerCount; i++)
            activePlayers.Add(i);

        tiedPlayers.Clear();

        if (anim != null)
        {
            anim.AnimateStartPanelOut();
            LeanTween.delayedCall(0.45f, () => StartRound());
        }
        else
        {
            StartRound();
        }
    }

    /// <summary>
    /// Called by the result panel button.
    /// If a tie was detected, replays with only the tied players; otherwise returns to the start panel.
    /// </summary>
    public void RestartGame()
    {
        if (tiedPlayers.Count > 1)
        {
            activePlayers = new List<int>(tiedPlayers);
            tiedPlayers.Clear();

            if (anim != null)
            {
                anim.AnimateResultPanelOut();
                LeanTween.delayedCall(0.36f, () => StartRound());
            }
            else
            {
                resultPanel.SetActive(false);
                StartRound();
            }
        }
        else
        {
            if (anim != null)
            {
                anim.AnimateResultPanelOut();
                LeanTween.delayedCall(0.36f, () =>
                {
                    resultPanel.SetActive(false);
                    startPanel.SetActive(true);
                    anim.AnimateStartPanelIn();
                });
            }
            else
            {
                resultPanel.SetActive(false);
                startPanel.SetActive(true);
            }
        }
    }

    public void OnSliderValueChanged()
    {
        sliderValueText.text = numberSlider.value.ToString();
    }

    public void SubmitGuess()
    {
        anim?.AnimateSubmitButtonBounce();

        int currentPlayer = playerOrder[currentTurnIndex];
        guesses[currentPlayer] = (int)numberSlider.value;
        currentTurnIndex++;

        if (currentTurnIndex >= activePlayers.Count)
        {
            EndGame();
        }
        else
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// Begins a new round using the current activePlayers list, picking a fresh target number.
    /// </summary>
    private void StartRound()
    {
        targetNumber = Random.Range(1, 101);
        currentTurnIndex = 0;
        guesses.Clear();

        playerOrder = new List<int>(activePlayers);
        ShuffleList(playerOrder);

        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        UpdateUI();

        anim?.AnimateGamePanelIn();
    }

    private void UpdateUI()
    {
        int currentPlayer = playerOrder[currentTurnIndex];
        playerTurnText.text = PlayerNames[currentPlayer];
        numberSlider.value = SliderDefaultValue;
        sliderValueText.text = SliderDefaultValue.ToString();

        anim?.AnimatePlayerTurnPunch();
    }

    /// <summary>
    /// Shuffles a list in-place using the Fisher-Yates algorithm.
    /// </summary>
    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void EndGame()
    {
        int minDifference = int.MaxValue;
        foreach (int player in activePlayers)
        {
            int diff = Mathf.Abs(targetNumber - guesses[player]);
            if (diff < minDifference)
                minDifference = diff;
        }

        List<int> winners = new List<int>();
        foreach (int player in activePlayers)
        {
            if (Mathf.Abs(targetNumber - guesses[player]) == minDifference)
                winners.Add(player);
        }

        if (winners.Count == 1)
        {
            tiedPlayers.Clear();
            resultText.text = "Bob pensait à " + targetNumber + ".\n" + PlayerNames[winners[0]] + " gagne !";
        }
        else
        {
            tiedPlayers = new List<int>(winners);

            List<string> winnerNames = new List<string>();
            foreach (int winner in winners)
                winnerNames.Add(PlayerNames[winner]);

            resultText.text = "Bob pensait à " + targetNumber + ".\nÉgalité entre " + string.Join(", ", winnerNames) + " !\nAppuyez sur Rejouer pour les départager.";
        }

        if (anim != null)
        {
            anim.AnimateGamePanelOut();
            LeanTween.delayedCall(0.45f, () =>
            {
                resultPanel.SetActive(true);
                anim.AnimateResultPanelIn();
            });
        }
        else
        {
            gamePanel.SetActive(false);
            resultPanel.SetActive(true);
        }
    }
}