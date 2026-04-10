using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;


public class MultiMiner : MonoBehaviour
{
    private const string fromRerollKey = "fromRerollKey";
    private const string fromDuelKey = "FromDuel";
    private const string duelWinnerKey = "DuelWinner";

    [Header("UI Elements")]
    public TextMeshProUGUI texteEcran;        // Texte principal (Score/Chrono)
    public GameObject panneauClassement;     // Le Panel qui contient le classement
    public TextMeshProUGUI texteClassement;   // Le texte de la liste finale
    public Button boutonPret;                // Le bouton pour Start / Joueur suivant
    public Button boutonReroll;              // Le bouton pour rejouer une partie différente

    [Header("Réglages Gameplay")]
    public float tempsDeJeu = 10f;           // Temps imparti par joueur
    public float seuilSensibilite = 15f;     // Force de la secousse (15 est une bonne base)
    public float delaiEntreCoups = 0.12f;    // Empêche de compter trop de coups d'un coup

    // Variables de jeu internes
    private float tempsRestant;
    private int scoreActuel;
    private bool jeuEnCours = false;
    private float prochainCoupAutorise;
    private Vector3 derniereAcceleration;

     // Gestion Dynamique des joueurs
    private List<string> nomsDesDuellistes = new List<string>();
    private int indexJoueurActuel = 0;

    struct Resultat
    {
        public string nom;
        public int score;
    }
    List<Resultat> historiqueResultats = new List<Resultat>();

    public void OnRerollClicked()
    {
        // autre jeu
        Debug.Log("Reroll cliqué, réinitialisation du jeu.");
        PlayerPrefs.SetInt(fromRerollKey, 1);
        PlayerPrefs.SetInt(fromDuelKey, 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
    void Awake()
    {
         // 1. On récupère le nombre exact de joueurs envoyés par le DuelManager
         // "DuelPlayerCount" est la clé que tu as définie dans ton DuelManager
        int count = PlayerPrefs.GetInt("DuelPlayerCount", 0);

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                 // On récupère Player_0, Player_1, Player_2, Player_3... à l'infini
                string nomDuJoueur = PlayerPrefs.GetString("DuelPlayer_" + i);
                nomsDesDuellistes.Add(nomDuJoueur);
            }
        }
        else
        {
             // Sécurité si tu testes la scène Miner sans passer par le HUB
            nomsDesDuellistes.Add("Joueur de Test 1");
            nomsDesDuellistes.Add("Joueur de Test 2");
        }
    }
    IEnumerator SpawnButton()
    {
        yield return new WaitForSeconds(5f);
        boutonReroll.gameObject.SetActive(true);
    }
    void Start()
    {
        panneauClassement.SetActive(false);
        boutonPret.onClick.AddListener(DemarrerManche);
        AfficherEcranAttente();
        boutonReroll.gameObject.SetActive(false);

    }

    void AfficherEcranAttente()
    {
        jeuEnCours = false;
        boutonPret.gameObject.SetActive(true);
        boutonPret.GetComponentInChildren<TextMeshProUGUI>().text = "JE SUIS PRÊT !";

        string nomAffiche = nomsDesDuellistes[indexJoueurActuel];
        // On affiche qui doit jouer
        texteEcran.text = $"AU TOUR DE :\n<color=#FFD700>{nomAffiche}</color>\n\nSecoue le plus vite possible !";
    }

    void DemarrerManche()
    {
        scoreActuel = 0;
        tempsRestant = tempsDeJeu;
        derniereAcceleration = Input.acceleration;
        prochainCoupAutorise = 0;

        boutonPret.gameObject.SetActive(false);
        panneauClassement.SetActive(false);
        jeuEnCours = true;
        StartCoroutine(SpawnButton());

    }

    void Update()
    {
        if (!jeuEnCours) return;

        tempsRestant -= Time.deltaTime;

        // --- DÉTECTION DE SECOUSSE FLUIDE (Delta-Velocity) ---
        Vector3 accelerationActuelle = Input.acceleration;
        float mouvementSoudain = (accelerationActuelle - derniereAcceleration).magnitude;
        derniereAcceleration = accelerationActuelle;

        // Si le mouvement est assez brusque et que le délai est respecté
        if (mouvementSoudain > (seuilSensibilite / 10f) && Time.time > prochainCoupAutorise)
        {
            scoreActuel++;
            prochainCoupAutorise = Time.time + delaiEntreCoups;

            // Vibration pour plus de sensation
#if !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

        // Mise à jour de l'UI pendant que le joueur secoue
        string nomAffiche = nomsDesDuellistes[indexJoueurActuel];
        texteEcran.text = $"{nomAffiche}\n<size=160%>{scoreActuel}</size> MINES\n<color=red>{tempsRestant:F1}s</color>";

        // Quand le temps est fini
        if (tempsRestant <= 0)
        {
            EnregistrerResultat();
        }
    }

    void EnregistrerResultat()
    {
        jeuEnCours = false;

        // Stocke le score du joueur actuel
        historiqueResultats.Add(new Resultat
        {
            nom = nomsDesDuellistes[indexJoueurActuel],
            score = scoreActuel
        });

        indexJoueurActuel++;

        // Est-ce qu'il reste quelqu'un dans la liste ?
        if (indexJoueurActuel < nomsDesDuellistes.Count)
        {
            // Oui -> Au suivant !
            AfficherEcranAttente();
        }
        else
        {
             // Non -> Classement final
            AfficherClassementFinal();
        }
    }

    void AfficherClassementFinal()
    {
        panneauClassement.SetActive(true);

        // TRI AUTOMATIQUE : Celui qui a le plus gros score est 1er
        var classementTrié = historiqueResultats.OrderByDescending(r => r.score).ToList();

        string renduTexte = "<color=yellow>--- RÉSULTATS DUEL ---</color>\n\n";
        for (int i = 0; i < classementTrié.Count; i++)
        {
             // On met le gagnant en vert
            string couleur = (i == 0) ? "green" : "white";
            renduTexte += $"<color={couleur}>{i + 1}. {classementTrié[i].nom} : {classementTrié[i].score} mines</color>\n";
        }

        texteClassement.text = renduTexte;
        texteEcran.text = "DUEL TERMINÉ !";

        // Configuration du bouton final
        boutonPret.gameObject.SetActive(true);
        boutonPret.GetComponentInChildren<TextMeshProUGUI>().text = "RETOUR AU HUB";

         // On nettoie le bouton et on lui donne une nouvelle mission : rentrer à la maison
        boutonPret.onClick.RemoveAllListeners();
        boutonPret.onClick.AddListener(() => {
            // On informe le HUB de qui a gagné (le 1er de la liste triée)
            PlayerPrefs.SetString(duelWinnerKey, classementTrié[0].nom);
            PlayerPrefs.SetInt(fromDuelKey, 1);
            PlayerPrefs.Save();

            // Charge ton plateau de jeu (vérifie le nom de ta scène)
            SceneManager.LoadScene("GameScene");
        });
    }
}