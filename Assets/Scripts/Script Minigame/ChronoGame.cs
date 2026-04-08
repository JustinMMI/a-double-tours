using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ChronoGame : MonoBehaviour
{
    private static readonly string[] DefaultPlayerNames = { "Épée", "Jetpack", "Ordinateur", "Casque" };

    public TextMeshProUGUI affichage;

    private float chrono = 0f;
    private bool tourne = false;
    private bool enAttente = true;

    private int joueurActuel = 1;
    private int totalJoueurs;
    private string[] playerNames;
    private List<float> scores = new List<float>();
    private bool jeuTermine = false;

    // La regle est construite dynamiquement car Generatedtime est initialise a l'execution.
    private string Regle => "<color=#FFFF00><size=120%>OBJECTIF : " + Generatedtime.ToString("F3") + " secondes !</size></color>\n--------------------------\n";

    void Start()
    {
        InitializePlayers();
        GenerateRandomTime();
        AfficherMessage(playerNames[0] + "\nCliquez pour LANCER");
    }

    /// <summary>Charge les joueurs connectés via PlayerPrefs ou utilise des valeurs par défaut.</summary>
    private void InitializePlayers()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);

        if (count < 2)
        {
            totalJoueurs = DefaultPlayerNames.Length;
            playerNames = (string[])DefaultPlayerNames.Clone();
            Debug.Log("[ChronoGame] Aucun joueur connecté, utilisation des noms par défaut.");
        }
        else
        {
            totalJoueurs = count;
            playerNames = new string[totalJoueurs];
            for (int i = 0; i < totalJoueurs; i++)
                playerNames[i] = PlayerPrefs.GetString("Player_" + i, DefaultPlayerNames[i]);
        }

        Debug.Log($"[ChronoGame] {totalJoueurs} joueur(s) : {string.Join(", ", playerNames)}");
    }

    public float Generatedtime { get; private set; } = 10f; // Valeur par défaut, sera remplacée par GenerateRandomTime()
    public void GenerateRandomTime()
    {
        Generatedtime = Random.Range(5, 15);
        Debug.Log("Temps cible généré : " + Generatedtime.ToString("F3") + " secondes");
    }

    void Update()
    {
        if (tourne)
        {
            chrono += Time.deltaTime;
            // Le chrono se cache après 3 secondes
            string tempsVisible = (chrono < 3f ? chrono.ToString("F3") : "???");
            AfficherMessage(playerNames[joueurActuel - 1] + "\n<size=150%>" + tempsVisible + "s</size>");
        }
    }

    public void ActionBouton()
    {
        if (jeuTermine)
        {
            RelancerTout();
            return;
        }

        if (enAttente)
        {
            enAttente = false;
            tourne = true;
            chrono = 0f;
        }
        else if (tourne)
        {
            ArreterTour();
        }
    }

    void ArreterTour()
    {
        tourne = false;
        scores.Add(chrono);

        if (joueurActuel < totalJoueurs)
        {
            joueurActuel++;
            enAttente = true;
            AfficherMessage("SCORE ENREGISTRÉ !\n\n" + playerNames[joueurActuel - 1] + "\nCliquez quand vous êtes PRÊT");
        }
        else
        {
            AfficherClassement();
        }
    }

    void AfficherClassement()
    {
        jeuTermine = true;

        var resultats = scores
            .Select((temps, index) => new {
                Nom = " - " + playerNames[index],
                Temps = temps,
                Ecart = Mathf.Abs(Generatedtime - temps)
            })
            .OrderBy(r => r.Ecart)
            .ToList();

        string[] couleurs = { "#FFD700", "#C0C0C0", "#CD7F32", "#FF4500" };
        string[] prefixes = { "1er", "2e", "3e", "4e" };

        string podium = "CLASSEMENT FINAL :\n";

        for (int i = 0; i < resultats.Count; i++)
        {
            string couleur = i < couleurs.Length ? couleurs[i] : "#FF4500";
            string prefixe = i < prefixes.Length ? prefixes[i] : $"{i + 1}e";
            podium += $"<color={couleur}>{prefixe}{resultats[i].Nom} : {resultats[i].Temps:F3}s (Ecart: {resultats[i].Ecart:F3}s)</color>\n";
        }

        podium += "\n<size=80%>Cliquez pour REJOUER</size>";
        AfficherMessage(podium);
    }

    void AfficherMessage(string contenu)
    {
        affichage.text = Regle + contenu;
    }

    void RelancerTout()
    {
        GenerateRandomTime();
        joueurActuel = 1;
        scores.Clear();
        chrono = 0f;
        jeuTermine = false;
        tourne = false;
        enAttente = true;
        AfficherMessage(playerNames[0] + "\nCliquez pour LANCER");
    }
}