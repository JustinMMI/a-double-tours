using UnityEngine;

public class GestionCible : MonoBehaviour
{
    public float tempsAffichage = 1.0f;

    void Start()
    {
        // La cible disparaît seule après un certain temps
        Destroy(gameObject, tempsAffichage);
    }

    void OnMouseDown()
    {
        // Affiche un message de victoire dans la console
        Debug.Log("GAGNÉ ! Le jeu s'arrête.");

        // --- LA MAGIE EST ICI ---
        // On fige le temps : tout s'arrête (les scripts, les apparitions, etc.)
        Time.timeScale = 0;

        // On détruit quand même la cible sur laquelle on a cliqué
        Destroy(gameObject);
    }
}