using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ChronoGame : MonoBehaviour
{
    public TextMeshProUGUI affichage;

    private float chrono = 0f;
    private bool tourne = false;
    private bool enAttente = true;

    private int joueurActuel = 1;
    private List<float> scores = new List<float>();
    private bool jeuTermine = false;

    // La règle affichée en permanence en haut
    private string regle = "<color=#FFFF00><size=120%>OBJECTIF : 10.000 secondes !</size></color>\n--------------------------\n";

    void Start()
    {
        AfficherMessage("JOUEUR 1\nCliquez pour LANCER");
    }

    void Update()
    {
        if (tourne)
        {
            chrono += Time.deltaTime;
            // Le chrono se cache après 3 secondes
            string tempsVisible = (chrono < 3f ? chrono.ToString("F3") : "???");
            AfficherMessage("JOUEUR " + joueurActuel + "\n<size=150%>" + tempsVisible + "s</size>");
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

        if (joueurActuel < 4)
        {
            joueurActuel++;
            enAttente = true;
            AfficherMessage("SCORE ENREGISTRE !\n\nJOUEUR " + joueurActuel + "\nCliquez quand vous etes PRET");
        }
        else
        {
            AfficherClassement();
        }
    }

    void AfficherClassement()
    {
        jeuTermine = true;

        // On crée une liste d'objets avec le nom, le temps et l'écart
        var resultats = scores
            .Select((temps, index) => new {
                Nom = " - J" + (index + 1),
                Temps = temps,
                Ecart = Mathf.Abs(10f - temps)
            })
            .OrderBy(r => r.Ecart) // Le plus proche de 10s en premier
            .ToList();

        string podium = "CLASSEMENT FINAL :\n";

        for (int i = 0; i < resultats.Count; i++)
        {
            string couleur;
            string prefixe;

            // Attribution des couleurs demandées
            if (i == 0) { couleur = "#FFD700"; prefixe = "1er"; }      // Jaune Or
            else if (i == 1) { couleur = "#C0C0C0"; prefixe = "2e"; }  // Gris Argent
            else if (i == 2) { couleur = "#CD7F32"; prefixe = "3e"; }  // Marron Bronze
            else { couleur = "#FF4500"; prefixe = "4e"; }              // Rouge

            podium += $"<color={couleur}>{prefixe}{resultats[i].Nom} : {resultats[i].Temps:F3}s (Ecart: {resultats[i].Ecart:F3}s)</color>\n";
        }

        podium += "\n<size=80%>Cliquez pour REJOUER</size>";
        AfficherMessage(podium);
    }

    void AfficherMessage(string contenu)
    {
        affichage.text = regle + contenu;
    }

    void RelancerTout()
    {
        joueurActuel = 1;
        scores.Clear();
        chrono = 0f;
        jeuTermine = false;
        tourne = false;
        enAttente = true;
        AfficherMessage("JOUEUR 1\nCliquez pour LANCER");
    }
}