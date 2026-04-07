using UnityEngine;
using TMPro; // Pour g�rer les textes
using UnityEngine.UI;
using System.Collections; // Indispensable pour le minuteur (Coroutine)

public class JeuMiroir : MonoBehaviour
{
    [Header("Configuration UI")]
    public TMP_Text sequenceAffichée;
    public TMP_InputField zoneSaisie;
    public TMP_Text scoreTexte;

    [Header("Réglages du Jeu")]
    public float tempsAffichage = 2.0f; // Temps avant que le texte disparaisse

    private string sequenceActuelle = "";
    private int score = 0;

    void Start()
    {
        // On pr�pare la zone de saisie pour qu'elle soit vide au d�but
        zoneSaisie.text = "";
        scoreTexte.text = "Score : 0";
        GenererNouvelleSequence();
    }

    public void GenererNouvelleSequence()
    {
        // 1. Cr�ation d'une suite de 4 lettres au hasard
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        sequenceActuelle = "";
        for (int i = 0; i < 4; i++)
        {
            sequenceActuelle += caracteres[Random.Range(0, caracteres.Length)];
        }

        // 2. On lance le chrono pour afficher puis cacher
        StopAllCoroutines(); // S�curit� : on arr�te les anciens chronos s'il y en a
        StartCoroutine(AfficherPuisCacher());
    }

    IEnumerator AfficherPuisCacher()
    {
        // On affiche la s�quence
        sequenceAffichée.text = "Bob dit : " + sequenceActuelle;
        zoneSaisie.text = ""; // On vide la zone de saisie pour le nouveau tour
        zoneSaisie.interactable = false; // D�sactive la saisie pendant qu'on montre la r�ponse

        // On attend le temps d�fini (ex: 2 secondes)
        yield return new WaitForSeconds(tempsAffichage);

        // On cache la s�quence et on laisse le joueur �crire
        sequenceAffichée.text = "À ton tour ! Recopie le code.";
        zoneSaisie.interactable = true;
        zoneSaisie.ActivateInputField(); // Met le curseur directement dans la case
    }

    public void VerifierReponse()
    {
        // ToUpper() sert � accepter "abc" m�me si Bob a dit "ABC"
        if (zoneSaisie.text.ToUpper() == sequenceActuelle)
        {
            score++;
            scoreTexte.text = "Score : " + score;
            GenererNouvelleSequence();
        }
        else
        {
            // Si c'est faux, on remet � z�ro
            sequenceAffichée.text = "PERDU ! C'était : " + sequenceActuelle;
            score = 0;
            scoreTexte.text = "Score : 0";
            // On attend un peu avant de relancer une partie pour que le joueur voit son erreur
            Invoke("GenererNouvelleSequence", 2.0f);
        }
    }
}