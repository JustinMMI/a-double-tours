using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class NumberSecret : MonoBehaviour
{
    public TextMeshProUGUI texteInfo;

    struct Choix
    {
        public int idJoueur;
        public int valeur;
    }

    private List<Choix> choixDuTour = new List<Choix>();
    private int[] scoresJoueurs = new int[3]; // Stocke les points de J1, J2 et J3
    private int nombreDeJoueurs = 3;
    private int tourActuel = 1;
    private int maxTours = 3;

    void Start()
    {
        System.Array.Clear(scoresJoueurs, 0, scoresJoueurs.Length);
        AfficherDebutTour();
    }

    void AfficherDebutTour()
    {
        choixDuTour.Clear();
        texteInfo.text = $"<color=orange>TOUR {tourActuel} / {maxTours}</color>\n" +
                         "Objectif : Le plus grand chiffre UNIQUE gagne ses points !\n\n" +
                         "Joueur 1, choisis ton chiffre...";
    }

    public void JouerChiffre(int chiffre)
    {
        int joueurQuiJoue = choixDuTour.Count + 1;
        choixDuTour.Add(new Choix { idJoueur = joueurQuiJoue, valeur = chiffre });

        if (choixDuTour.Count < nombreDeJoueurs)
        {
            int prochain = choixDuTour.Count + 1;
            texteInfo.text = $"Tour {tourActuel} : Chiffre enregistré !\n\n" +
                             $"<color=yellow>Joueur {prochain}</color>, à toi !";
        }
        else
        {
            CalculerTour();
        }
    }

    void CalculerTour()
    {
        // 1. Trouver les chiffres uniques du tour
        var groupes = choixDuTour.GroupBy(c => c.valeur);
        var uniques = groupes.Where(g => g.Count() == 1)
                             .Select(g => g.First())
                             .ToList();

        string bilanTour = "";

        if (uniques.Count > 0)
        {
            // 2. Le plus grand gagne ses points
            var gagnantDuTour = uniques.OrderByDescending(c => c.valeur).First();
            scoresJoueurs[gagnantDuTour.idJoueur - 1] += gagnantDuTour.valeur;

            bilanTour = $"Le <color=green>Joueur {gagnantDuTour.idJoueur}</color> gagne <color=green>+{gagnantDuTour.valeur}</color> points !";
        }
        else
        {
            bilanTour = "Doublons partout ! Personne ne marque de points.";
        }

        // Affichage des scores actuels
        string tableauScores = $"\n\nSCORES :\nJ1: {scoresJoueurs[0]} pts | J2: {scoresJoueurs[1]} pts | J3: {scoresJoueurs[2]} pts";

        if (tourActuel < maxTours)
        {
            texteInfo.text = $"FIN DU TOUR {tourActuel}\n{bilanTour}{tableauScores}";
            tourActuel++;
            Invoke("AfficherDebutTour", 4f);
        }
        else
        {
            AfficherGagnantFinal(bilanTour, tableauScores);
        }
    }

    void AfficherGagnantFinal(string dernierBilan, string tableauScores)
    {
        int maxScore = scoresJoueurs.Max();
        int indexGagnant = System.Array.IndexOf(scoresJoueurs, maxScore) + 1;

        texteInfo.text = $"<color=red>FIN DE LA PARTIE !</color>\n{dernierBilan}{tableauScores}\n\n" +
                         $"<size=120%>VICTOIRE FINALE : <color=yellow>JOUEUR {indexGagnant}</color> !</size>";

        // Relance une nouvelle partie après 8 secondes
        tourActuel = 1;
        Invoke("Start", 8f);
    }
}