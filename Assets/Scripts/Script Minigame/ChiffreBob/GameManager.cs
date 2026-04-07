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

    private int targetNumber;
    private int currentTurnIndex = 0;

    private List<int> activePlayers = new List<int>();

    private List<int> playerOrder = new List<int>();

    private Dictionary<int, int> guesses = new Dictionary<int, int>();

    private List<int> tiedPlayers = new List<int>();

    void Start()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void StartGame()
    {
        activePlayers.Clear();
        for (int i = 0; i < TotalPlayerCount; i++)
            activePlayers.Add(i);

        tiedPlayers.Clear();
        StartRound();
    }

    public void RestartGame()
    {
        resultPanel.SetActive(false);

        if (tiedPlayers.Count > 1)
        {
            activePlayers = new List<int>(tiedPlayers);
            tiedPlayers.Clear();
            StartRound();
        }
        else
        {
            startPanel.SetActive(true);
        }
    }

    public void OnSliderValueChanged()
    {
        sliderValueText.text = numberSlider.value.ToString();
    }

    public void SubmitGuess()
    {
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
    }

    private void UpdateUI()
    {
        int currentPlayer = playerOrder[currentTurnIndex];
        playerTurnText.text = PlayerNames[currentPlayer];
        numberSlider.value = SliderDefaultValue;
        sliderValueText.text = SliderDefaultValue.ToString();
    }

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
        gamePanel.SetActive(false);
        resultPanel.SetActive(true);

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
    }
}