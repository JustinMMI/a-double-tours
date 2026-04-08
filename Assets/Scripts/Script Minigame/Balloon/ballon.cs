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

    private Vector3 originalPosition;
    private bool isshriking = false;

    private void Start()
    {

        originalPosition = transform.position;
        button1.onClick.AddListener(() =>
        {
            transform.localScale += Vector3.one * airAdded;
        });
        StartCoroutine(AirLossVariation(2.5f));

        returnButton.gameObject.SetActive(false);
        victoryText.text = ""; 
    }

    private void Update()
    {
        if (isshriking) return;

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

    private void WhosWinner()
    {
        if (isshriking) return;
        airLoss = 0f;
        isshriking = true;

        string winnerName = ownerPlayerName == balloonManager.player1Text.text
            ? balloonManager.player2Text.text
            : balloonManager.player1Text.text;

        if (victoryText != null)
            victoryText.text = "Le gagnant est " + winnerName + " il a gagné  rajouté une récompense";

        returnButton.gameObject.SetActive(true);
        button1.gameObject.SetActive(false);

        returnButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene");
        });

        balloonManager.NotifyOpponent(this);
    }

    public void OnOpponentWon()
    {
        isshriking = true;
        airLoss = 0f;
        button1.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }


    private IEnumerator AirLossVariation(float interval)
    {
        yield return new WaitForSeconds(interval);
        if (!isshriking)
        {
            airLoss = Random.Range(50f, 220f);
            StartCoroutine(AirLossVariation(interval));
        }
    }
}