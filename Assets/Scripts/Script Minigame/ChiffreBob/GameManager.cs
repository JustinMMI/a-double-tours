using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static readonly string[] DefaultPlayerNames = { "Épée", "Jetpack", "Ordinateur", "Casque" };
    private const int SliderDefaultValue = 50;

    private string[] playerNames;
    private int totalPlayerCount;

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject resultPanel;
    public Button button;

    public TMP_Text playerTurnText;
    public Slider numberSlider;
    public TMP_Text sliderValueText;
    public TMP_Text resultText;

    public BobChiffreAnim anim;

    private int targetNumber;
    private int currentTurnIndex = 0;
    private List<int> activePlayers = new List<int>();
    private List<int> playerOrder = new List<int>();
    private Dictionary<int, int> guesses = new Dictionary<int, int>();
    private List<int> tiedPlayers = new List<int>();

    void Start()
    {
        InitializePlayers();

        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        resultPanel.SetActive(false);

        anim?.AnimateStartPanelIn();
    }

    /// <summary>Charge les joueurs connectés via PlayerPrefs ou utilise des valeurs par défaut.</summary>
    private void InitializePlayers()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);

        if (count < 2)
        {
            totalPlayerCount = DefaultPlayerNames.Length;
            playerNames = (string[])DefaultPlayerNames.Clone();
            Debug.Log("[ChiffreBob] Aucun joueur connecté, utilisation des noms par défaut.");
        }
        else
        {
            totalPlayerCount = count;
            playerNames = new string[totalPlayerCount];
            for (int i = 0; i < totalPlayerCount; i++)
                playerNames[i] = PlayerPrefs.GetString("Player_" + i, DefaultPlayerNames[i]);
        }

        Debug.Log($"[ChiffreBob] {totalPlayerCount} joueur(s) : {string.Join(", ", playerNames)}");
    }

    public void StartGame()
    {
        activePlayers.Clear();
        for (int i = 0; i < totalPlayerCount; i++)
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
        playerTurnText.text = playerNames[currentPlayer];
        numberSlider.value = SliderDefaultValue;
        sliderValueText.text = SliderDefaultValue.ToString();

        anim?.AnimatePlayerTurnPunch();
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private const string WinnerKey = "DuelWinner";
    private const string FromDuelKey = "FromDuel";

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
            resultText.text = "Bob pensait à " + targetNumber + ".\n" + playerNames[winners[0]] + " gagne !";
            button.gameObject.SetActive(false);
            PlayerPrefs.SetString(WinnerKey, playerNames[winners[0]]);
            PlayerPrefs.SetInt(FromDuelKey, 1);
            StartCoroutine(SendingToMain());
        }
        else
        {
            tiedPlayers = new List<int>(winners);

            List<string> winnerNames = new List<string>();
            foreach (int winner in winners)
                winnerNames.Add(playerNames[winner]);

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
        }}

        private IEnumerator SendingToMain()
        {
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene("GameScene");
    }
}
