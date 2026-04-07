using UnityEngine;
using UnityEngine.UI;
using TMPro;
// Cette ligne permet d'utiliser le nouveau système de touches d'Unity
using UnityEngine.InputSystem;

public class Fill : MonoBehaviour
{
    [Header("Configuration UI")]
    public Image jaugeVerte;
    public TextMeshProUGUI messageTexte;

    [Header("Réglages du Jeu")]
    public float vitesse = 0.5f; // Vitesse de remplissage

    private float progression = 0f;
    private bool jeuEnCours = true;

    void Start()
    {
        // On s'assure que le texte est vide au début
        if (messageTexte != null) messageTexte.text = "Appuie sur ESPACE au milieu !";
        progression = 0f;
    }

    void Update()
    {
        if (jeuEnCours)
        {
            // 1. Faire avancer la barre
            progression += Time.deltaTime * vitesse;
            if (jaugeVerte != null) jaugeVerte.fillAmount = progression;

            // 2. Détecter le clic ou la touche Espace (Nouveau Système)
            bool toucheEspace = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            bool clicSouris = Pointer.current != null && Pointer.current.press.wasPressedThisFrame;

            if (toucheEspace || clicSouris)
            {
                VerifierVictoire();
            }

            // 3. Si on dépasse 100%, on a perdu
            if (progression >= 1f)
            {
                TerminerJeu("Trop tard ! C'est plein.");
            }
        }
    }

    void VerifierVictoire()
    {
        // On calcule l'écart entre la progression et le milieu (0.5)
        // Mathf.Abs donne la valeur absolue (toujours positive)
        float score = Mathf.Abs(progression - 0.5f);

        // Si l'écart est très petit (moins de 0.05), c'est gagné
        if (score <= 0.05f)
        {
            TerminerJeu("ÉGALITÉ PARFAITE !");
        }
        else
        {
            TerminerJeu("Raté ! Trop loin du centre.");
        }
    }

    void TerminerJeu(string message)
    {
        jeuEnCours = false;
        if (messageTexte != null) messageTexte.text = message;

        // Petit bonus : affiche le score final en console
        Debug.Log("Jeu terminé : " + message + " Score : " + progression);
    }
}