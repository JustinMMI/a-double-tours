using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ChronoGame : MonoBehaviour
{
    public TextMeshProUGUI affichage;

    private float chrono = 0f;
    private bool tourne = false;

    private int joueurActuel = 1;
    private List<float> scores = new List<float>(); // Liste pour stocker les chronos
    private bool jeuTermine = false;

    void Update()
    {
        if (tourne)
        {
            chrono += Time.deltaTime;
            affichage.text = "Joueur " + joueurActuel + "\n" + (chrono < 3f ? chrono.ToString("F2") : "???");
        }
    }

    public void ActionBouton()
    {
        if (jeuTermine)
        {
            RelancerTout();
            return;
        }

        if (!tourne)
        {
            tourne = true;
        }
        else
        {
            ArreterTour();
        }
    }

    void ArreterTour()
    {
        tourne = false;
        scores.Add(chrono); // On enregistre le temps

        if (joueurActuel < 4)
        {
            joueurActuel++;
            chrono = 0f;
            affichage.text = "Score enregistre !\nAu tour du Joueur " + joueurActuel;
        }
        else
        {
            AfficherClassement();
        }
    }

    void AfficherClassement()
    {
        jeuTermine = true;

        // On calcule l'écart avec 10s pour chaque score
        var resultats = scores
            .Select((temps, index) => new { Nom = "J" + (index + 1), Ecart = Mathf.Abs(10f - temps), Temps = temps })
            .OrderBy(r => r.Ecart) // Le plus petit écart gagne
            .ToList();

        string texteFinal = "CLASSEMENT FINAL :\n";
        for (int i = 0; i < resultats.Count; i++)
        {
            texteFinal += (i + 1) + ". " + resultats[i].Nom + " (" + resultats[i].Temps.ToString("F2") + "s)\n";
        }

        texteFinal += "\nCliquez pour rejouer";
        affichage.text = texteFinal;
    }

    void RelancerTout()
    {
        joueurActuel = 1;
        scores.Clear();
        chrono = 0f;
        jeuTermine = false;
        tourne = false;
        affichage.text = "Joueur 1 : Pret ?";
    }
}