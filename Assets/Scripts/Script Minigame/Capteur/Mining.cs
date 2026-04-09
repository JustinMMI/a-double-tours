using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MultiMiner : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI texteEcran;       // Texte principal (Score/Chrono)
    public GameObject panneauClassement;    // Le Panel qui contient le classement
    public TextMeshProUGUI texteClassement;  // Le texte de la liste finale
    public Button boutonPret;               // Le bouton pour Start / Joueur suivant

    [Header("Réglages Gameplay")]
    public float tempsDeJeu = 10f;          // Temps imparti par joueur
    public float seuilSensibilite = 15f;    // Force de la secousse (15 est une bonne base)
    public float delaiEntreCoups = 0.12f;   // Empêche de compter trop de coups d'un coup

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

    private void Awake()
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

    void Start()
    {
        panneauClassement.SetActive(false);
        boutonPret.onClick.AddListener(DemarrerManche);
        AfficherEcranAttente();
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

        texteClassement.text = renduTexte; // Affichage du classement final
        texteEcran.text = "DUEL TERMINÉ !"; // Message final

        // Configuration du bouton final
        boutonPret.gameObject.SetActive(true); // On réutilise le même bouton pour retourner au HUB
        boutonPret.GetComponentInChildren<TextMeshProUGUI>().text = "RETOUR AU HUB"; // On change le texte du bouton pour indiquer la nouvelle action

        // On nettoie le bouton et on lui donne une nouvelle mission : rentrer à la maison
        boutonPret.onClick.RemoveAllListeners(); // On enlève les anciens listeners pour éviter les conflits
        boutonPret.onClick.AddListener(() => { // Action à faire quand on clique pour retourner au HUB
            // On informe le HUB de qui a gagné (le 1er de la liste triée)
            PlayerPrefs.SetString("DuelWinner", classementTrié[0].nom); // Stocke le nom du gagnant pour que le HUB puisse l'afficher ou lui donner une récompense
            PlayerPrefs.SetInt("FromDuel", 1); // Indique au HUB que le joueur vient de terminer un duel (utile pour déclencher des événements spécifiques dans le HUB)
            PlayerPrefs.Save(); // Sauvegarde immédiate des données pour s'assurer qu'elles sont disponibles dès que le HUB les demandera

            // Charge ton plateau de jeu (vérifie le nom de ta scène)
            SceneManager.LoadScene("GameScene"); // Remplace "GameScene" par le nom exact de ta scène de HUB
        });
    }
}