using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    public TextMeshProUGUI bobText;
    public Button nextButton;
    public ObstacleGenerator obsGen;
    public Button duelButton;

    void Start()
    {
        if (PlayerPrefs.GetInt("FromDuel", 0) == 1)
        {
            string winnerName = PlayerPrefs.GetString("DuelWinner");
            bobText.text = "BOB : 'Le duel est terminé ! Le sort en a décidé ainsi : le vainqueur est " + winnerName + " !'";
            PlayerPrefs.SetInt("FromDuel", 0);
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            ShowObstacles();
            duelButton.gameObject.SetActive(false);
        }
    }

    void ShowObstacles()
    {
        obsGen.GenerateObstacles();
        bobText.text = obsGen.sentenceObstacle;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(ShowStartPositions);
    }

    void ShowStartPositions()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);
        string msg = "BOB : 'Bien. Maintenant, placez vos pions en bas des tours :'\n\n";

        for (int i = 0; i < count; i++)
        {
            string pName = PlayerPrefs.GetString("Player_" + i);
            int start = (Random.value > 0.5f) ? 1 : 16;
            string tName = (start == 1) ? "Tour 1" : "Tour 2";

            msg += "- " + pName + " -> " + tName + "\n";
        }

        bobText.text = msg;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(FinalStart);
    }

    void FinalStart()
    {
        bobText.text = "BOB : 'Le jeu commence. Que la souffrance soit avec vous !'";
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(End);
    }

    void End()
    {
        bobText.text = "";
        nextButton.gameObject.SetActive(false);
        duelButton.gameObject.SetActive(true);
    }

    [Header("Events Settings")]
    public string[] randomEvents = {
};

    public void OnBobClicked()
    {
        int randomIndex = Random.Range(0, randomEvents.Length);

        while (randomIndex == lastEventIndex)
        {
            randomIndex = Random.Range(0, randomEvents.Length);
        }

        lastEventIndex = randomIndex;

        int EventOrNot = Random.Range(0, 100);
        if (EventOrNot <= 60)
        {
            bobText.text = "BOB : 'Aucun événement aléatoire cette fois-ci...'";
            return;
        }
        else
        {
        bobText.text = randomEvents[randomIndex];
        }
    }

    private int lastEventIndex = -1;

    public void OnDuelClicked()
    {
        SceneManager.LoadScene("DuelMenu");
    }

    public void OnQuitClicked()
    {
        SceneManager.LoadScene("Main");
    }

}