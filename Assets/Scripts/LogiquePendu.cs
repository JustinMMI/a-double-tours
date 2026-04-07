using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LogiquePendu : MonoBehaviour
{
    [Header("Configuration")]
    // Voici ta liste de mots. Tu peux en ajouter autant que tu veux dans Unity !
    public string[] listeDeMots = { "BANANE", "POMME", "SOLEIL", "ORDINATEUR", "UNITY", "BOB", "PENDU", "PROJET" };

    [Header("Interface UI")]
    public TMP_Text affichageMot;
    public TMP_InputField champSaisie;
    public TMP_Text texteMessage;
    public TMP_Text texteLettresFausses;

    private string motA_Deviner;
    private string motCache = "";

    // On sépare les vies
    private int viesJ1 = 5;
    private int viesJ2 = 5;

    private int tentativesBob = 0;
    private int joueurActuel = 1;
    private List<string> lettresRatees = new List<string>();

    void Start()
    {
        // On choisit un mot au hasard dans la liste
        int indexAleatoire = Random.Range(0, listeDeMots.Length);
        motA_Deviner = listeDeMots[indexAleatoire].ToUpper();

        // Initialisation de l'affichage
        motCache = "";
        for (int i = 0; i < motA_Deviner.Length; i++) motCache += "_ ";

        affichageMot.text = motCache;
        texteLettresFausses.text = "Ratés : ";
        MettreAJourMessage("Le duel commence !");
    }

    public void ValiderProposition()
    {
        string proposition = champSaisie.text.ToUpper().Trim();
        champSaisie.text = "";
        champSaisie.ActivateInputField();

        if (string.IsNullOrEmpty(proposition)) return;

        // Anti-doublon
        if (proposition.Length == 1)
        {
            if (lettresRatees.Contains("<s>" + proposition + "</s>") || motCache.Contains(proposition))
            {
                MettreAJourMessage("Déjà fait !");
                return;
            }
        }

        if (proposition.Length > 1) // Tente le MOT
        {
            if (proposition == motA_Deviner) Gagner();
            else
            {
                ActiverRegleBob();
                ChangerDeJoueur();
            }
        }
        else // Tente une LETTRE
        {
            VerifierLettre(proposition[0]);
        }
    }

    void VerifierLettre(char lettre)
    {
        if (motA_Deviner.Contains(lettre.ToString()))
        {
            ActualiserAffichageMot(lettre);
            MettreAJourMessage("Bien joué !");
            if (!motCache.Contains("_")) Gagner();
        }
        else
        {
            AjouterLettreRatee(lettre.ToString());

            if (tentativesBob > 0)
            {
                tentativesBob--;
                MettreAJourMessage("Raté ! Bob vous protège.");
            }
            else
            {
                // On retire la vie au joueur qui joue actuellement
                if (joueurActuel == 1) viesJ1--;
                else viesJ2--;

                if (viesJ1 <= 0 || viesJ2 <= 0) Perdre();
                else
                {
                    MettreAJourMessage("Raté !");
                    ChangerDeJoueur();
                }
            }
        }
    }

    void AjouterLettreRatee(string l)
    {
        lettresRatees.Add("<s>" + l + "</s>");
        texteLettresFausses.text = "Ratés : " + string.Join(" ", lettresRatees);
    }

    void ActualiserAffichageMot(char lettre)
    {
        string nouveauRendu = "";
        for (int i = 0; i < motA_Deviner.Length; i++)
        {
            if (motA_Deviner[i] == lettre || (motCache.Length > i * 2 && motCache[i * 2] != '_'))
                nouveauRendu += motA_Deviner[i] + " ";
            else
                nouveauRendu += "_ ";
        }
        motCache = nouveauRendu;
        affichageMot.text = motCache;
    }

    void ActiverRegleBob()
    {
        tentativesBob = 2;
        MettreAJourMessage("FAUX ! Bob offre 2 essais à l'adversaire.");
    }

    void ChangerDeJoueur()
    {
        joueurActuel = (joueurActuel == 1) ? 2 : 1;
        MettreAJourMessage("Au tour du Joueur " + joueurActuel);
    }

    void MettreAJourMessage(string msg)
    {
        texteMessage.text = msg + "\n<color=blue>J1: " + viesJ1 + " vies</color> | <color=red>J2: " + viesJ2 + " vies</color>";
    }

    void Gagner()
    {
        affichageMot.text = motA_Deviner;
        texteMessage.text = "VICTOIRE DU JOUEUR " + joueurActuel + " !";
        champSaisie.interactable = false;
    }

    void Perdre()
    {
        int gagnant = (viesJ1 <= 0) ? 2 : 1;
        texteMessage.text = "JOUEUR " + gagnant + " GAGNE ! (L'autre n'a plus de vies)\nLe mot était : " + motA_Deviner;
        champSaisie.interactable = false;
    }
}