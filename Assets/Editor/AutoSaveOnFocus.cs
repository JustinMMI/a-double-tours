using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DuelMashing : MonoBehaviour
{
    public Slider barreDuel;

    [Header("FORCE DES COUPS")]
    public float forceImpact = 15f; // On passe à 15 pour que ça bouge VRAIMENT !

    private float scoreGlobal = 50f;

    void Start()
    {
        if (barreDuel != null)
        {
            barreDuel.minValue = 0;
            barreDuel.maxValue = 100;
            barreDuel.value = 50f;
            barreDuel.interactable = false;
        }
    }

    void Update()
    {
        if (barreDuel == null || Keyboard.current == null) return;

        // JOUEUR BLEU (Touche A)
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            scoreGlobal -= forceImpact;
            Debug.Log("BAM ! BLEU : " + scoreGlobal);
        }

        // JOUEUR ROUGE (Touche P)
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            scoreGlobal += forceImpact;
            Debug.Log("BOOM ! ROUGE : " + scoreGlobal);
        }

        // Mise à jour DIRECTE (pas de lerp, pas d'attente)
        scoreGlobal = Mathf.Clamp(scoreGlobal, 0, 100);
        barreDuel.value = scoreGlobal;

        // Vérification Victoire
        if (scoreGlobal <= 0) { Debug.Log("BLEU WIN"); this.enabled = false; }
        if (scoreGlobal >= 100) { Debug.Log("ROUGE WIN"); this.enabled = false; }
    }
}