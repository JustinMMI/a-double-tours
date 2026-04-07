using UnityEngine;
using TMPro; // Pour gérer les textes
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
        // On prépare la zone de saisie pour qu'elle soit vide au début
        zoneSaisie.text = "";
        scoreTexte.text = "Score : 0";
        GenererNouvelleSequence();
    }

    public void GenererNouvelleSequence()
    {
        // 1. Création d'une suite de 4 lettres au hasard
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        sequenceActuelle = "";
        for (int i = 0; i < 4; i++)
        {
            sequenceActuelle += caracteres[Random.Range(0, caracteres.Length)];
        }

        // 2. On lance le chrono pour afficher puis cacher
        StopAllCoroutines(); // Sécurité : on arrête les anciens chronos s'il y en a
        StartCoroutine(AfficherPuisCacher());
    }

    IEnumerator AfficherPuisCacher()
    {
        // On affiche la séquence
        sequenceAffichée.text = "Bob dit : " + sequenceActuelle;
        zoneSaisie.text = ""; // On vide la zone de saisie pour le nouveau tour
        zoneSaisie.interactable = false; // Désactive la saisie pendant qu'on montre la réponse

        // On attend le temps défini (ex: 2 secondes)
        yield return new WaitForSeconds(tempsAffichage);

        // On cache la séquence et on laisse le joueur écrire
        sequenceAffichée.text = "À ton tour ! Recopie le code.";
        zoneSaisie.interactable = true;
        zoneSaisie.ActivateInputField(); // Met le curseur directement dans la case
    }

    public void VerifierReponse()
    {
        // ToUpper() sert à accepter "abc" même si Bob a dit "ABC"
        if (zoneSaisie.text.ToUpper() == sequenceActuelle)
        {
            score++;
            scoreTexte.text = "Score : " + score;
            GenererNouvelleSequence();
        }
        else
        {
            // Si c'est faux, on remet à zéro
            sequenceAffichée.text = "PERDU ! C'était : " + sequenceActuelle;
            score = 0;
            scoreTexte.text = "Score : 0";
            // On attend un peu avant de relancer une partie pour que le joueur voit son erreur
            Invoke("GenererNouvelleSequence", 2.0f);
        }
    }
}