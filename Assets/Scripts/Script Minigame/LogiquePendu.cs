using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LogiquePendu : MonoBehaviour
{
    [Header("Configuration")]
    public string[] listeDeMots = { "BANANE", "POMME", "SOLEIL", "ORDINATEUR", "UNITY", "BOB", "PENDU", "PROJET" };

    [Header("Interface UI")]
    public TMP_Text affichageMot;
    public TMP_InputField champSaisie;
    public TMP_Text texteMessage;
    public TMP_Text texteLettresFausses;

    private string motA_Deviner;
    private string motCache = "";
    private int viesJ1 = 5;
    private int viesJ2 = 5;
    private int boucliersActifs = 0;
    private int joueurActuel = 1;
    private List<string> lettresRatees = new List<string>();

    void Start()
    {
        NouvellePartie();
    }

    void NouvellePartie()
    {
        // Choisir un mot au hasard
        int indexAleatoire = Random.Range(0, listeDeMots.Length);
        motA_Deviner = listeDeMots[indexAleatoire].ToUpper();

        // Créer les tirets _ _ _
        motCache = "";
        for (int i = 0; i < motA_Deviner.Length; i++) motCache += "_ ";

        affichageMot.text = motCache;
        lettresRatees.Clear();
        viesJ1 = 5;
        viesJ2 = 5;
        boucliersActifs = 0;
        joueurActuel = 1;
        texteLettresFausses.text = "Ratees : ";

        MettreAJourMessage("--- DEBUT DU DUEL ---");
    }

    public void ValiderProposition()
    {
        string proposition = champSaisie.text.ToUpper().Trim();
        champSaisie.text = "";
        champSaisie.ActivateInputField();

        if (string.IsNullOrEmpty(proposition)) return;

        // --- CAS 1 : TENTATIVE DE MOT COMPLET ---
        if (proposition.Length > 1)
        {
            if (proposition == motA_Deviner)
            {
                Gagner();
            }
            else
            {
                // Erreur fatale sur le mot
                ActiverRegleBob();
                MettreAJourMessage("!!! MAUVAIS MOT !!! (+1 Bouclier pour l'autre)");
                ChangerDeJoueur();
            }
        }
        // --- CAS 2 : TENTATIVE D'UNE SEULE LETTRE ---
        else
        {
            char lettre = proposition[0];

            // Verifier si déjà joué
            if (lettresRatees.Contains("<s>" + lettre + "</s>") || motCache.Contains(lettre.ToString()))
            {
                MettreAJourMessage("[!] Lettre deja utilisee");
                return;
            }

            // Bonne lettre
            if (motA_Deviner.Contains(lettre.ToString()))
            {
                ActualiserAffichageMot(lettre);
                if (!motCache.Contains("_"))
                {
                    Gagner();
                }
                else
                {
                    MettreAJourMessage("BIEN JOUE ! Tu rejoues.");
                }
            }
            // Mauvaise lettre
            else
            {
                VerifierEchec(lettre);
            }
        }
    }

    void VerifierEchec(char lettre)
    {
        AjouterLettreRatee(lettre.ToString());

        // Si le joueur a un bouclier (Règle de Bob)
        if (boucliersActifs > 0)
        {
            boucliersActifs--;
            MettreAJourMessage("RATE ! (Bouclier Bob utilise)");
            // On ne change pas de joueur, le bouclier sauve le tour
        }
        else
        {
            // Perte de vie
            if (joueurActuel == 1) viesJ1--;
            else viesJ2--;

            if (viesJ1 <= 0 || viesJ2 <= 0)
            {
                Perdre();
            }
            else
            {
                // On change de joueur car c'est une erreur sans bouclier
                joueurActuel = (joueurActuel == 1) ? 2 : 1;
                MettreAJourMessage("MAUVAISE LETTRE !");
            }
        }
    }

    void ActiverRegleBob()
    {
        boucliersActifs = 1;
    }

    void ChangerDeJoueur()
    {
        joueurActuel = (joueurActuel == 1) ? 2 : 1;
    }

    void MettreAJourMessage(string info)
    {
        string tourCouleur = (joueurActuel == 1) ? "#3399FF" : "#FF3333";
        string bouclierInfo = (boucliersActifs > 0) ? "\n[BOUCLIER ACTIF]" : "";

        texteMessage.text = "<b>" + info + "</b>\n" +
                           "TOUR : <color=" + tourCouleur + ">JOUEUR " + joueurActuel + "</color>" +
                           "\n<color=#3399FF>J1: " + viesJ1 + " HP</color> | <color=#FF3333>J2: " + viesJ2 + " HP</color>" +
                           "<b><color=yellow>" + bouclierInfo + "</color></b>";
    }

    void AjouterLettreRatee(string l)
    {
        lettresRatees.Add("<s>" + l + "</s>");
        texteLettresFausses.text = "Ratees : " + string.Join(" ", lettresRatees);
    }

    void ActualiserAffichageMot(char lettre)
    {
        string nouveauRendu = "";
        for (int i = 0; i < motA_Deviner.Length; i++)
        {
            if (motA_Deviner[i] == lettre || (motCache.Length > i * 2 && motCache[i * 2] != '_'))
                nouveauRendu += motA_Deviner[i] + " ";
            else nouveauRendu += "_ ";
        }
        motCache = nouveauRendu;
        affichageMot.text = motCache;
    }

    void Gagner()
    {
        affichageMot.text = motA_Deviner;
        texteMessage.text = "--- VICTOIRE JOUEUR " + joueurActuel + " ---";
        champSaisie.interactable = false;
    }

    void Perdre()
    {
        int gagnant = (viesJ1 <= 0) ? 2 : 1;
        texteMessage.text = "--- FIN DE PARTIE ---\nLE JOUEUR " + gagnant + " GAGNE !\nLe mot : " + motA_Deviner;
        champSaisie.interactable = false;
    }
}