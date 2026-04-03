using UnityEngine;
using UnityEngine.UI;

public class DuelMashing : MonoBehaviour
{
    public Slider barreDuel;

    [Header("Réglages")]
    public float scoreGlobal = 50f;
    public float forceImpact = 10f; // Puissance d'un clic sur le bouton

    void Start()
    {
        if (barreDuel != null)
        {
            barreDuel.minValue = 0;
            barreDuel.maxValue = 100;
            barreDuel.value = scoreGlobal;
            barreDuel.interactable = false;
        }
    }

    // FONCTION POUR LE BOUTON BLEU
    public void ClickBleu()
    {
        ModifierScore(-forceImpact);
        Debug.Log("Bouton BLEU cliqué !");
    }

    // FONCTION POUR LE BOUTON ROUGE
    public void ClickRouge()
    {
        ModifierScore(forceImpact);
        Debug.Log("Bouton ROUGE cliqué !");
    }

    void ModifierScore(float valeur)
    {
        scoreGlobal += valeur;
        scoreGlobal = Mathf.Clamp(scoreGlobal, 0, 100);
        barreDuel.value = scoreGlobal;

        if (scoreGlobal <= 0) { Debug.Log("VICTOIRE BLEUE"); FinDeMatch(); }
        if (scoreGlobal >= 100) { Debug.Log("VICTOIRE ROUGE"); FinDeMatch(); }
    }

    void FinDeMatch()
    {
        // Optionnel : On peut désactiver les boutons ici pour ne plus cliquer
        Debug.Log("Le match est terminé !");
    }
}