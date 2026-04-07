using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DuelMashing : MonoBehaviour
{
    [Header("Interface UI")]
    public Slider barreDuel;
    public TextMeshProUGUI texteInfo; // Le texte qui affiche les règles PUIS le gagnant
    public Button boutonBleu;
    public Button boutonRouge;
    public GameObject boutonRejouer;

    [Header("Réglages")]
    public float scoreGlobal = 50f;
    public float forceImpact = 10f;

    void Start()
    {
        InitialiserJeu();
    }

    public void InitialiserJeu()
    {
        scoreGlobal = 50f;
        barreDuel.value = scoreGlobal;

        // On affiche les règles (et elles resteront là pendant tout le duel)
        texteInfo.text = "RÈGLES : Cliquez le plus vite possible pour gagner !";

        boutonRejouer.SetActive(false);
        ActiverBoutonsJeu(true);
    }

    // --- ACTIONS DES BOUTONS ---

    public void ClickBleu()
    {
        ModifierScore(-forceImpact);
    }

    public void ClickRouge()
    {
        ModifierScore(forceImpact);
    }

    // --- LOGIQUE ---

    void ModifierScore(float valeur)
    {
        // Si le jeu est fini, on ne fait plus rien
        if (boutonRejouer.activeSelf) return;

        scoreGlobal += valeur;
        scoreGlobal = Mathf.Clamp(scoreGlobal, 0, 100);
        barreDuel.value = scoreGlobal;

        if (scoreGlobal <= 0) TerminerPartie("L'ÉQUIPE BLEUE A GAGNÉ !");
        if (scoreGlobal >= 100) TerminerPartie("L'ÉQUIPE ROUGE A GAGNÉ !");
    }

    void TerminerPartie(string messageGagnant)
    {
        // Le texte des règles est ENFIN remplacé par le message de victoire
        texteInfo.text = messageGagnant;

        boutonRejouer.SetActive(true);
        ActiverBoutonsJeu(false);
    }

    void ActiverBoutonsJeu(bool etat)
    {
        boutonBleu.interactable = etat;
        boutonRouge.interactable = etat;
    }
}