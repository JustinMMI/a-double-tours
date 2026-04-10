using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DuelMashing : MonoBehaviour
{
    [Header("Interface UI")]
    public Slider barreDuel;
    public TextMeshProUGUI texteInfo;
    public TextMeshProUGUI texteJoueurBleu;
    public TextMeshProUGUI texteJoueurRouge;
    public Button boutonBleu;
    public Button boutonRouge;
    public Button boutonRejouer;

    [Header("Réglages")]
    public float scoreGlobal = 50f;
    public float forceImpact = 10f;

    private const string ReturnSceneName = "GameScene";
    private const string WinnerKey = "DuelWinner";
    private const string FromDuel = "FromDuel";

    private string nomJoueurBleu = "Joueur Bleu";
    private string nomJoueurRouge = "Joueur Rouge";

    void Start()
    {
        InitializePlayers();
        InitialiserJeu();
    }






    private void InitializePlayers()
    {
        int count = PlayerPrefs.GetInt("DuelPlayerCount", 0);

        if (count >= 2)
        {
            nomJoueurBleu = PlayerPrefs.GetString("DuelPlayer_0", "Joueur Bleu");
            nomJoueurRouge = PlayerPrefs.GetString("DuelPlayer_1", "Joueur Rouge");
        }
    }

    public void InitialiserJeu()
    {
        boutonBleu.gameObject.SetActive(true);
        boutonRouge.gameObject.SetActive(true);
        scoreGlobal = 50f;
        barreDuel.value = scoreGlobal;

        texteInfo.text = "RÈGLES : Cliquez le plus vite possible pour gagner !";

        if (texteJoueurBleu != null) texteJoueurBleu.text = nomJoueurBleu;
        if (texteJoueurRouge != null) texteJoueurRouge.text = nomJoueurRouge;

        boutonRejouer.gameObject.SetActive(false);
    }

    public void ClickBleu()
    {
        ModifierScore(-forceImpact);
    }

    public void ClickRouge()
    {
        ModifierScore(forceImpact);
    }

    void ModifierScore(float valeur)
    {
        if (boutonRejouer.gameObject.activeSelf) return;

        scoreGlobal += valeur;
        scoreGlobal = Mathf.Clamp(scoreGlobal, 0, 100);
        barreDuel.value = scoreGlobal;

        if (scoreGlobal <= 0) TerminerLEPartie(nomJoueurBleu);
        if (scoreGlobal >= 100) TerminerLEPartie(nomJoueurRouge);
    }

    public void TerminerLEPartie(string nomGagnant)
    {
        texteInfo.text = $"{nomGagnant} A GAGNÉ !";

        PlayerPrefs.SetString(WinnerKey, nomGagnant);
        PlayerPrefs.SetInt(FromDuel, 1);
        PlayerPrefs.Save();

        boutonRejouer.gameObject.SetActive(true);
        boutonBleu.gameObject.SetActive(false);
        boutonRouge.gameObject.SetActive(false);
    }
    public void RetournerAuJeu()
    {
        SceneManager.LoadScene(ReturnSceneName);
    }
}