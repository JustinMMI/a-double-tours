// ballon.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;

public class ballon : MonoBehaviour
{
    [Header("Bouton")]
    public Button button1;

    [Header("Paramètres")]
    public float airAdded = 50f;
    public float airLoss = 50f;
    public BalloonManager balloonManager;
    public Button returnButton;

    [Header("Identité")]
    public string ownerPlayerName = BalloonManager.FallbackPlayer1;

    [Header("UI Résultat")]
    public TextMeshProUGUI victoryText;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    public float gameDuration = 15f;

    private Vector3 originalPosition;
    private bool isshriking = false;
    private float timer;
    private bool timerStarted = false;

    [Header("Panel d'introduction")]
    public GameObject introPanel;
    public Button quitterPanelButton;

    private const string WinnerKey = "DuelWinner";
    private const string FromDuel = "FromDuel";

    private void Start()
    {
        originalPosition = transform.position;
        timer = gameDuration;
        timerStarted = false;

        button1.interactable = false;

        if (introPanel != null) introPanel.SetActive(true);

        if (quitterPanelButton != null)
            quitterPanelButton.onClick.AddListener(LancerJeu);

        returnButton.gameObject.SetActive(false);
        victoryText.text = "";
    }

    private void Update()
    {
        if (isshriking) return;

        if (!timerStarted) return;

        timer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timer).ToString();

        if (timer <= 0f)
        {
            balloonManager.ResolveBySize();
            return;
        }

        transform.localScale -= Vector3.one * Time.deltaTime * airLoss;

        if (transform.localScale.x >= 300)
        {
            float intensity = (transform.localScale.x - 300f) / 200f;
            transform.position = originalPosition + new Vector3(
                Random.Range(-0.2f, 0.2f) * intensity,
                Random.Range(-0.2f, 0.2f) * intensity,
                0
            );
        }
        else
        {
            transform.position = originalPosition;
        }

        if (transform.localScale.x <= 0)
        {
            WhosWinner();
            return;
        }

        if (transform.localScale.x >= 500)
        {
            WhosWinner();
            return;
        }
    }

    private void LancerJeu()
    {
        if (introPanel != null) introPanel.SetActive(false);

        timerStarted = true;
        button1.interactable = true;

        button1.onClick.AddListener(() =>
        {
            transform.localScale += Vector3.one * airAdded;
            StartCoroutine(AirLossVariation(2.5f));
        });
    }

    private void WhosWinner()
    {
        if (isshriking) return;
        airLoss = 0f;
        isshriking = true;

        string winnerName = ownerPlayerName == balloonManager.player1Text.text
            ? balloonManager.player2Text.text
            : balloonManager.player1Text.text;

        PlayerPrefs.SetString(WinnerKey, winnerName);
        PlayerPrefs.SetInt(FromDuel, 1);

        if (victoryText != null)
            victoryText.text = "Le gagnant est " + winnerName + " !";

        returnButton.gameObject.SetActive(true);
        button1.gameObject.SetActive(false);
        gameObject.SetActive(false);

        returnButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));

        balloonManager.NotifyOpponent(this);
    }

    public void DeclareWinner(string winnerName)
    {
        if (isshriking) return;
        airLoss = 0f;
        isshriking = true;

        PlayerPrefs.SetString(WinnerKey, winnerName);
        PlayerPrefs.SetInt(FromDuel, 1);

        if (victoryText != null)
            victoryText.text = "Le gagnant est " + winnerName + " !";

        returnButton.gameObject.SetActive(true);
        button1.gameObject.SetActive(false);
        gameObject.SetActive(false);

        returnButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
    }

    public void OnOpponentWon()
    {
        isshriking = true;
        airLoss = 0f;
        button1.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public float GetSize() => transform.localScale.x;

    private IEnumerator AirLossVariation(float interval)
    {
        yield return new WaitForSeconds(interval);
        if (!isshriking)
        {
            Debug.Log("cc");
            airLoss = Random.Range(50f, 220f);
            StartCoroutine(AirLossVariation(interval));
        }
    }
}