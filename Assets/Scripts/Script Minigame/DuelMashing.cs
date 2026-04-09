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
    public GameObject boutonRejouer;

    [Header("Réglages")]
    public float scoreGlobal = 50f;
    public float forceImpact = 10f;

    private const string ReturnSceneName = "GameScene";
    private const string DuelWinnerKey = "DuelWinner";
    private const string FromDuelKey = "FromDuel";

    private string nomJoueurBleu = "Joueur Bleu";
    private string nomJoueurRouge = "Joueur Rouge";

    void Start()
    {
        ChargerJoueurs();
        InitialiserJeu();
    }






    private void ChargerJoueurs()
    {
        int duelPlayerCount = PlayerPrefs.GetInt("DuelPlayerCount", 0);

        if (duelPlayerCount >= 2)
        {
            nomJoueurBleu = PlayerPrefs.GetString("DuelPlayer_0", "Joueur Bleu");
            nomJoueurRouge = PlayerPrefs.GetString("DuelPlayer_1", "Joueur Rouge");
        }
        else
        {
            Debug.LogWarning("[DuelMashing] Aucun joueur de duel trouvé dans PlayerPrefs. Noms par défaut utilisés.");
        }

        Debug.Log($"[DuelMashing] Joueur Bleu : {nomJoueurBleu} | Joueur Rouge : {nomJoueurRouge}");
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

        boutonRejouer.SetActive(false);
        AppuiyeBoutonsJeu(true);
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
        if (boutonRejouer.activeSelf) return;

        scoreGlobal += valeur;
        scoreGlobal = Mathf.Clamp(scoreGlobal, 0, 100);
        barreDuel.value = scoreGlobal;

        if (scoreGlobal <= 0) TerminerLEPartie(nomJoueurBleu);
        if (scoreGlobal >= 100) TerminerLEPartie(nomJoueurRouge);
    }

    void TerminerLEPartie(string nomGagnant)
    {
        texteInfo.text = $"{nomGagnant} A GAGNÉ !";

        PlayerPrefs.SetString(DuelWinnerKey, nomGagnant);
        PlayerPrefs.SetInt(FromDuelKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"[DuelMashing] Vainqueur : {nomGagnant}");

        boutonRejouer.SetActive(true);
        AppuiyeBoutonsJeu(false);
        boutonBleu.gameObject.SetActive(false);
        boutonRouge.gameObject.SetActive(false);
    }

    public void RetournerAuJeu()
    {
        SceneManager.LoadScene(ReturnSceneName);
    }

    void AppuiyeBoutonsJeu(bool etat)
    {
        boutonBleu.interactable = etat;
        boutonRouge.interactable = etat;
    }
}