using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement; // Pour retourner au HUB

public class MultiMiner : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI texteEcran;
    public GameObject panneauClassement;
    public TextMeshProUGUI texteClassement;
    public Button boutonPret;

    [Header("Réglages")]
    public float tempsInitial = 10f;
    public int scoreObjectif = 30;

    private float tempsRestant;
    private int scoreActuel;
    private bool jeuEnCours = false;
    private bool peutFrapper = true;

    // --- SYSTÈME AUTOMATIQUE ---
    private List<string> nomsDesDuellistes = new List<string>();
    private int indexJoueurActuel = 0;

    struct Resultat
    {
        public string nom;
        public float temps;
        public bool succes;
    }
    List<Resultat> resultats = new List<Resultat>();

    void Awake()
    {
        // 1. On récupère AUTOMATIQUEMENT les joueurs envoyés par le DuelManager
        int count = PlayerPrefs.GetInt("DuelPlayerCount", 0);
        if (count == 0)
        {
            Debug.LogError("Aucun joueur trouvé dans PlayerPrefs !");
            nomsDesDuellistes.Add("Joueur 1");
            nomsDesDuellistes.Add("Joueur 2");
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                nomsDesDuellistes.Add(PlayerPrefs.GetString("DuelPlayer_" + i));
            }
        }
    }

    void Start()
    {
        panneauClassement.SetActive(false);
        boutonPret.onClick.AddListener(DemarrerManche);
        AfficherPretSuivant();
    }

    void AfficherPretSuivant()
    {
        texteEcran.text = $"À TON TOUR :\n<color=yellow>{nomsDesDuellistes[indexJoueurActuel]}</color>";
        boutonPret.GetComponentInChildren<TextMeshProUGUI>().text = "JE SUIS PRÊT !";
    }

    void DemarrerManche()
    {
        scoreActuel = 0;
        tempsRestant = tempsInitial;
        jeuEnCours = true;
        boutonPret.gameObject.SetActive(false);
        panneauClassement.SetActive(false);
    }

    void Update()
    {
        if (!jeuEnCours) return;

        tempsRestant -= Time.deltaTime;
        float force = Input.acceleration.magnitude;

        if (force > 2.5f && peutFrapper)
        {
            scoreActuel++;
            peutFrapper = false;
        }
        if (force < 1.8f) peutFrapper = true;

        texteEcran.text = $"{nomsDesDuellistes[indexJoueurActuel]}\nScore: {scoreActuel}/{scoreObjectif}\nTemps: {tempsRestant:F1}s";

        if (scoreActuel >= scoreObjectif || tempsRestant <= 0)
        {
            FinDeManche(scoreActuel >= scoreObjectif);
        }
    }

    void FinDeManche(bool victoire)
    {
        jeuEnCours = false;
        resultats.Add(new Resultat
        {
            nom = nomsDesDuellistes[indexJoueurActuel],
            temps = tempsInitial - tempsRestant,
            succes = victoire
        });

        indexJoueurActuel++;

        if (indexJoueurActuel < nomsDesDuellistes.Count)
        {
            // Joueur suivant
            boutonPret.gameObject.SetActive(true);
            AfficherPretSuivant();
        }
        else
        {
            // Tout le monde a fini
            TerminerLeJeuComplet();
        }
    }

    void TerminerLeJeuComplet()
    {
        panneauClassement.SetActive(true);
        // Tri : Les gagnants d'abord, puis les plus rapides
        var classement = resultats.OrderByDescending(r => r.succes).ThenBy(r => r.temps).ToList();

        string str = "FIN DU DUEL !\n\n";
        for (int i = 0; i < classement.Count; i++)
        {
            str += $"{i + 1}. {classement[i].nom} : {(classement[i].succes ? classement[i].temps.ToString("F2") + "s" : "ÉCHEC")}\n";
        }
        texteClassement.text = str;

        // --- RETOUR AUTOMATIQUE AU HUB ---
        boutonPret.gameObject.SetActive(true);
        boutonPret.GetComponentInChildren<TextMeshProUGUI>().text = "RETOURNER AU HUB";
        boutonPret.onClick.RemoveAllListeners();
        boutonPret.onClick.AddListener(() => {
            // On informe qui a gagné pour le HUB (Bob)
            PlayerPrefs.SetString("DuelWinner", classement[0].nom);
            PlayerPrefs.SetInt("FromDuel", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameScene"); // Mets ici le nom de ta scène HUB
        });
    }
}