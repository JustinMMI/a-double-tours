using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GenerateurCibles : MonoBehaviour
{
    [Header("Réglages Jeu")]
    public GameObject prefabCible;
    public float intervalle = 1.2f;

    [Header("Interface UI")]
    public GameObject panneauFin;
    public TextMeshProUGUI texteClassement;

    // Static pour garder les infos entre les rechargements
    private static int numeroJoueur = 1;
    private static Dictionary<int, float> scores = new Dictionary<int, float>();
    private static bool jeuEnCours = false; // Pour savoir si on doit lancer les cibles

    void Start()
    {
        // Si le jeu n'a pas encore commencé (Attente du bouton PRÊT)
        if (!jeuEnCours)
        {
            Time.timeScale = 0;
            panneauFin.SetActive(true);
            Debug.Log("--- ATTENTE JOUEUR " + numeroJoueur + " ---");
        }
        else
        {
            // Le jeu est lancé ! On cache tout et on génère
            Time.timeScale = 1;
            panneauFin.SetActive(false);
            InvokeRepeating("Apparition", 0.5f, intervalle);
            Debug.Log("--- JEU LANCÉ POUR J" + numeroJoueur + " ---");
        }
    }

    void Apparition()
    {
        Vector2 pos = new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f));
        Instantiate(prefabCible, pos, Quaternion.identity);
    }

    public void RecevoirScore(float score)
    {
        scores[numeroJoueur] = score;
        jeuEnCours = false; // Le jeu s'arrête
        Time.timeScale = 0;
        panneauFin.SetActive(true);
        AfficherResultats(score);
    }

    void AfficherResultats(float scoreActuel)
    {
        string recap = $"SCORE J{numeroJoueur} : {scoreActuel.ToString("F2")}s\n\n<b>CLASSEMENT :</b>\n";
        var tri = scores.OrderBy(x => x.Value);
        int rang = 1;
        foreach (var s in tri)
        {
            recap += $"{rang}. J{s.Key} : {s.Value.ToString("F2")}s\n";
            rang++;
        }
        texteClassement.text = recap;
    }

    public void JoueurSuivantPret()
    {
        // Si on a déjà un score pour ce joueur, on passe au numéro suivant
        if (scores.ContainsKey(numeroJoueur))
        {
            numeroJoueur++;
        }

        jeuEnCours = true; // On autorise le lancement des cibles
        Debug.Log("Bouton cliqué ! Lancement...");

        // On cache le panneau IMMÉDIATEMENT
        panneauFin.SetActive(false);
        Time.timeScale = 1;

        // On recharge la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}