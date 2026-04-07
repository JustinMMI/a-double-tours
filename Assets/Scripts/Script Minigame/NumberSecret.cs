using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class NumberSecret : MonoBehaviour
{
    public TextMeshProUGUI texteInfo;

    // Une petite structure pour lier un joueur à son score
    struct Choix
    {
        public int idJoueur;
        public int valeur;
    }

    private List<Choix> tousLesChoix = new List<Choix>();
    private int nombreDeJoueurs = 3;

    void Start()
    {
        AfficherObjectif();
    }

    void AfficherObjectif()
    {
        tousLesChoix.Clear();
        texteInfo.text = "OBJECTIF : Le plus grand chiffre UNIQUE gagne !\n\n" +
                         "<color=yellow>Joueur 1</color>, choisis ton chiffre...";
    }

    public void JouerChiffre(int chiffre)
    {
        // On enregistre qui a joué quoi
        int numeroDuJoueurActuel = tousLesChoix.Count + 1;
        tousLesChoix.Add(new Choix { idJoueur = numeroDuJoueurActuel, valeur = chiffre });

        if (tousLesChoix.Count < nombreDeJoueurs)
        {
            int prochain = tousLesChoix.Count + 1;
            texteInfo.text = "C'est noté !\n\n<color=yellow>Joueur " + prochain + "</color>, à toi !";
        }
        else
        {
            CalculerGagnant();
        }
    }

    void CalculerGagnant()
    {
        // 1. On compte les doublons
        var groupes = tousLesChoix.GroupBy(c => c.valeur);

        // 2. On ne garde que les choix qui sont apparus une seule fois
        var choixUniques = groupes.Where(g => g.Count() == 1)
                                  .Select(g => g.First())
                                  .ToList();

        if (choixUniques.Count > 0)
        {
            // 3. On trouve le choix avec la valeur la plus haute
            var gagnant = choixUniques.OrderByDescending(c => c.valeur).First();

            texteInfo.text = "RÉSULTAT :\n" +
                             "Le <color=green>Joueur " + gagnant.idJoueur + "</color> gagne !\n" +
                             "Son chiffre " + gagnant.valeur + " était le plus grand unique.";
        }
        else
        {
            texteInfo.text = "ÉGALITÉ :\nTout le monde a fait des doublons !\nLe château est sauf.";
        }

        // Relance après 5 secondes
        Invoke("AfficherObjectif", 5f);
    }
}