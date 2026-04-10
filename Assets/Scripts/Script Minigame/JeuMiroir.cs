using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
public class JeuMiroir : MonoBehaviour
{
    [Header("Configuration UI")]
    public TMP_Text sequenceAffichée;
    public TMP_Text saisieTexte;
    public TMP_Text scoreTexte;
    public Canvas CanvaInfo;
    public Canvas CanvaJeu;
    public Button LaunchGame;


    [Header("Réglages du Jeu")]
    public float tempsAffichage = 2.0f;
    public float delaiEntreJoueurs = 2.0f;

    private const string WinnerKey = "DuelWinner";
    private const string FromDuelKey = "FromDuel";
    private const string ReturnSceneName = "GameScene";
    private const int SequenceLongueurDepart = 2;

    private List<string> joueurs = new List<string>();
    private Dictionary<string, int> scores = new Dictionary<string, int>();

    private int joueurActuelIndex = 0;
    private string sequenceActuelle = "";
    private string saisieActuelle = "";
    private int scoreActuel = 0;
    private int longueurSequence = SequenceLongueurDepart;
    private bool saisieActive = false;

    private void Start()
    {
        CanvaInfo.enabled = true;
        CanvaJeu.enabled = false;

        if (sequenceAffichée == null || saisieTexte == null || scoreTexte == null)
        {
            Debug.LogError("[JeuMiroir] Un ou plusieurs champs UI ne sont pas assignés dans l'Inspector.", this);
            return;
        }
        LaunchGame.onClick.AddListener(StartGame);
    }
    private void StartGame()
    {
        CanvaInfo.enabled = false;
        CanvaJeu.enabled = true;
                ChargerJoueurs();
        saisieActuelle = "";
        RefreshSaisie();
        scoreTexte.text = "Score : 0";
        GenererNouvelleSequence();
    }

    private void ChargerJoueurs()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);

        if (count == 0)
        {
            Debug.LogWarning("[JeuMiroir] Aucun joueur trouvé dans PlayerPrefs. Joueurs par défaut utilisés.");
            joueurs.Add("Joueur 1");
            joueurs.Add("Joueur 2");
        }
        else
        {
            for (int i = 0; i < count; i++)
                joueurs.Add(PlayerPrefs.GetString("Player_" + i, "Joueur " + (i + 1)));
        }

        foreach (string j in joueurs)
            scores[j] = 0;

        Debug.Log($"[JeuMiroir] {joueurs.Count} joueur(s) : {string.Join(", ", joueurs)}");
    }

    public void GenererNouvelleSequence()
    {
        const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        sequenceActuelle = "";
        for (int i = 0; i < longueurSequence; i++)
            sequenceActuelle += caracteres[Random.Range(0, caracteres.Length)];

        longueurSequence++;
        StopAllCoroutines();
        StartCoroutine(AfficherPuisCacher());
    }

    private IEnumerator AfficherPuisCacher()
    {
        sequenceAffichée.text = "Bob dit : " + sequenceActuelle;
        saisieActuelle = "";
        RefreshSaisie();
        saisieActive = false;

        yield return new WaitForSeconds(tempsAffichage);

        string nomJoueur = joueurs[joueurActuelIndex];
        sequenceAffichée.text = $"{nomJoueur}, à ton tour ! Recopie le code.";
        saisieActive = true;
    }

    public void AppuyerLettre(string lettre)
    {
        if (!saisieActive) return;
        if (saisieActuelle.Length >= sequenceActuelle.Length) return;

        saisieActuelle += lettre;
        RefreshSaisie();

        if (saisieActuelle.Length == sequenceActuelle.Length)
            VerifierReponse();
    }

    public void Effacer()
    {
        if (!saisieActive || saisieActuelle.Length == 0) return;
        saisieActuelle = saisieActuelle.Substring(0, saisieActuelle.Length - 1);
        RefreshSaisie();
    }

    public void Valider()
    {
        if (!saisieActive || saisieActuelle.Length == 0) return;
        VerifierReponse();
    }

    private void RefreshSaisie()
    {
        saisieTexte.text = saisieActuelle;
    }

    private void VerifierReponse()
    {
        if (saisieActuelle.ToUpper() == sequenceActuelle)
        {
            scoreActuel++;
            scoreTexte.text = "Score : " + scoreActuel;
            GenererNouvelleSequence();
        }
        else
        {
            saisieActive = false;
            string nomJoueur = joueurs[joueurActuelIndex];
            scores[nomJoueur] = scoreActuel;
            sequenceAffichée.text = $"PERDU ! C'était : {sequenceActuelle}";

            joueurActuelIndex++;

            if (joueurActuelIndex >= joueurs.Count)
            {
                Invoke(nameof(EndGame), delaiEntreJoueurs);
                return;
            }

            scoreActuel = 0;
            longueurSequence = SequenceLongueurDepart;
            scoreTexte.text = "Score : 0";
            Invoke(nameof(GenererNouvelleSequence), delaiEntreJoueurs);
        }
    }

    private void EndGame()
    {
        CancelInvoke();
        StopAllCoroutines();
        saisieActive = false;

        var tri = scores.OrderByDescending(x => x.Value).ToList();
        int topScore = tri[0].Value;

        List<string> winners = tri
            .Where(x => x.Value == topScore)
            .Select(x => x.Key)
            .ToList();

        string winnerLabel = string.Join(", ", winners);
        PlayerPrefs.SetString(WinnerKey, winnerLabel);
        PlayerPrefs.SetInt(FromDuelKey, 1);
        PlayerPrefs.Save();
        Debug.Log($"[JeuMiroir] Vainqueur(s) : {winnerLabel}");

        string[] couleurs = { "#FFD700", "#C0C0C0", "#CD7F32", "#FF4500" };
        string result = "<b>Résultats :</b>\n\n";
        int rang = 1, i = 0;

        while (i < tri.Count)
        {
            int score = tri[i].Value;
            List<string> groupe = tri
                .Skip(i)
                .TakeWhile(x => x.Value == score)
                .Select(x => x.Key)
                .ToList();

            string c = (rang - 1) < couleurs.Length ? couleurs[rang - 1] : "#FFFFFF";
            result += $"<color={c}>{rang}. {string.Join(" & ", groupe)}  —  {score} pt(s)</color>\n";

            i += groupe.Count;
            rang += groupe.Count;
        }

        sequenceAffichée.text = result;
        Invoke(nameof(RetournerAuJeu), 5.0f);
    }

    public void RetournerAuJeu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(ReturnSceneName);
    }
}
