// ballon.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ballon : MonoBehaviour
{
    [Header("Bouton")]
    public Button button1;

    [Header("Paramètres")]
    public float airAdded = 50f;
    public float airLoss = 50f;
    public BalloonManager balloonManager;

    [Header("Identité")]
    public string ownerPlayerName = BalloonManager.FallbackPlayer1;

    [Header("UI Résultat")]
    public TextMeshProUGUI victoryText;

    private Vector3 originalPosition;
    private bool isDestroyed = false;

    private void Start()
    {
        // Fix 5: explicit match check with clear fallback
        if (balloonManager.player1Text.text == ownerPlayerName)
            ownerPlayerName = balloonManager.player1Text.text;
        else if (balloonManager.player2Text.text == ownerPlayerName)
            ownerPlayerName = balloonManager.player2Text.text;

        originalPosition = transform.position;
        button1.onClick.AddListener(() =>
        {
            transform.localScale += Vector3.one * airAdded;
        });
        StartCoroutine(AirLossVariation(2.5f));
    }

    private void Update()
    {
        if (isDestroyed) return;

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
            AnnounceLoser();
            return;
        }

        if (transform.localScale.x >= 500)
        {
            AnnounceLoser();
            return;
        }
    }

    private void AnnounceLoser()
    {
        if (isDestroyed) return;
        airLoss = 0f;
        isDestroyed = true;
        StartCoroutine(ReloadSceneAfterDelay());
        string winnerName = ownerPlayerName == balloonManager.player1Text.text
            ? balloonManager.player2Text.text
            : balloonManager.player1Text.text;



        if (victoryText != null)
            victoryText.text = "Le gagnant est " + winnerName;

        Destroy(gameObject);
    }

    private IEnumerator AirLossVariation(float interval)
    {
        yield return new WaitForSeconds(interval);
        if (!isDestroyed)
        {
            airLoss = Random.Range(50f, 220f);
            StartCoroutine(AirLossVariation(interval));
        }
    }

    private IEnumerator ReloadSceneAfterDelay()
    {
        Debug.Log("cc");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");

    }
}