using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenerateurCibles : MonoBehaviour
{
    public Button ButtonQuitter;
    public Button ButtonRejouer;
    private static readonly string[] DefaultPlayerNames = { "Épée", "Jetpack", "Ordinateur", "Casque" };

    private const string WinnerKey = "DuelWinner";
    private const string FromDuelKey = "FromDuel";

    [Header("Réglages Jeu")]
    public GameObject prefabCible;
    public float intervalle = 1.2f;

    [Header("Interface UI")]
    public GameObject panneauFin;
    public TextMeshProUGUI texteClassement;

    private static int numeroJoueur = 1;
    private static int totalJoueurs = 0;
    private static string[] playerNames;
    private static Dictionary<int, float> scores = new Dictionary<int, float>();
    private static bool jeuEnCours = false;

    void Start()
    {
        ButtonQuitter.gameObject.SetActive(false);
        ButtonRejouer.gameObject.SetActive(true);

        if (totalJoueurs == 0)
            InitializePlayers();

        if (!jeuEnCours)
        {
            Time.timeScale = 0;
            panneauFin.SetActive(true);
            texteClassement.text = $"Joueur suivant : <b>{playerNames[numeroJoueur - 1]}</b>\nÊtes-vous prêt ?";
            Debug.Log($"[CiblesGame] Attente joueur {numeroJoueur} ({playerNames[numeroJoueur - 1]})");
        }
        else
        {
            Time.timeScale = 1;
            panneauFin.SetActive(false);
            InvokeRepeating(nameof(Apparition), 0.5f, intervalle);
            Debug.Log($"[CiblesGame] Jeu lancé pour {playerNames[numeroJoueur - 1]}");
        }
    }

    private void InitializePlayers()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);

        if (count < 2)
        {
            totalJoueurs = DefaultPlayerNames.Length;
            playerNames = (string[])DefaultPlayerNames.Clone();
        }
        else
        {
            totalJoueurs = count;
            playerNames = new string[totalJoueurs];
            for (int i = 0; i < totalJoueurs; i++)
                playerNames[i] = PlayerPrefs.GetString("Player_" + i, DefaultPlayerNames[i]);
        }

        Debug.Log($"[CiblesGame] {totalJoueurs} joueur(s) : {string.Join(", ", playerNames)}");
    }

    private static void ResetStatics()
    {
        numeroJoueur = 1;
        totalJoueurs = 0;
        playerNames = null;
        scores.Clear();
        jeuEnCours = false;
    }

    void Apparition()
    {
        Vector2 pos = new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f));
        Instantiate(prefabCible, pos, Quaternion.identity);
    }

    public void RecevoirScore(float score)
    {
        scores[numeroJoueur] = score;
        jeuEnCours = false;
        Time.timeScale = 0;
        panneauFin.SetActive(true);
        AfficherResultats(score);
    }

    void AfficherResultats(float scoreActuel)
    {
        string recap = $"Score de <b>{playerNames[numeroJoueur - 1]}</b> : {scoreActuel:F2}s\n\n<b>CLASSEMENT :</b>\n";

        var tri = scores.OrderBy(x => x.Value).ToList();
        string[] couleurs = { "#FFD700", "#C0C0C0", "#CD7F32", "#FF4500" };

        for (int i = 0; i < tri.Count; i++)
        {
            string couleur = i < couleurs.Length ? couleurs[i] : "#FFFFFF";
            string nom = playerNames[tri[i].Key - 1];
            recap += $"<color={couleur}>{i + 1}. {nom} : {tri[i].Value:F2}s</color>\n";
        }

        texteClassement.text = recap;

        bool tousOntJoue = scores.Count >= totalJoueurs;
        ButtonQuitter.gameObject.SetActive(tousOntJoue);
        ButtonRejouer.gameObject.SetActive(!tousOntJoue);
    }

    public void JoueurSuivantPret()
    {
        if (scores.ContainsKey(numeroJoueur))
            numeroJoueur++;

        if (scores.Count >= totalJoueurs)
        {
            var gagnant = scores.OrderBy(x => x.Value).First();
            string nomGagnant = playerNames[gagnant.Key - 1];

            PlayerPrefs.SetString(WinnerKey, nomGagnant);
            PlayerPrefs.SetInt(FromDuelKey, 1);
            Debug.Log($"[CiblesGame] Vainqueur : {nomGagnant}");

            Time.timeScale = 1;
            ResetStatics();
            SceneManager.LoadScene("GameScene");
            return;
        }

        jeuEnCours = true;
        panneauFin.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}