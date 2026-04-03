using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DuelMashing : MonoBehaviour
{
    [Header("Interface UI")]
    public Slider barreDuel;
    public TextMeshProUGUI texteInfo; // Le texte qui affiche les r�gles PUIS le gagnant
    public Button boutonBleu;
    public Button boutonRouge;
    public GameObject boutonRejouer;

    [Header("R�glages")]
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

        // On affiche les r�gles (et elles resteront l� pendant tout le duel)
        texteInfo.text = "R�GLES : Cliquez le plus vite possible pour gagner !";

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

        if (scoreGlobal <= 0) TerminerPartie("L'�QUIPE BLEUE A GAGN� !");
        if (scoreGlobal >= 100) TerminerPartie("L'�QUIPE ROUGE A GAGN� !");
    }

    void TerminerPartie(string messageGagnant)
    {
        // Le texte des r�gles est ENFIN remplac� par le message de victoire
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