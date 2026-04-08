using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChronoGame : MonoBehaviour
{
    private static readonly string[] DefaultPlayerNames = { "Épée", "Jetpack", "Ordinateur", "Casque" };

    public TextMeshProUGUI affichage;
    public Button button1;
    public Button button2;

    private float chrono = 0f;
    private bool tourne = false;
    private bool enAttente = true;

    private int joueurActuel = 1;
    private int totalJoueurs;
    private string[] playerNames;
    private List<float> scores = new List<float>();

    private string Regle => "<color=#FFFF00><size=120%>OBJECTIF : " + Generatedtime.ToString("F3") + " secondes !</size></color>\n--------------------------\n";

    void Start()
    {
        InitializePlayers();
        GenerateRandomTime();
        AfficherMessage(playerNames[0] + "\nCliquez pour LANCER");
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(false);
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
    }

    public float Generatedtime { get; private set; } = 10f;

    public void GenerateRandomTime()
    {
        Generatedtime = Random.Range(5, 15);
    }

    void Update()
    {
        if (tourne)
        {
            chrono += Time.deltaTime;
            string tempsVisible = (chrono < 3f ? chrono.ToString("F3") : "???");
            AfficherMessage(playerNames[joueurActuel - 1] + "\n<size=150%>" + tempsVisible + "s</size>");
        }
    }

    public void ActionBouton()
    {
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

        podium += "\n<size=80%>Cliquez pour Quitter</size>";
        AfficherMessage(podium);
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(true);
        button2.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
    }

    void AfficherMessage(string contenu)
    {
        affichage.text = Regle + contenu;
    }
}