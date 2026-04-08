using UnityEngine;

public class GestionCible : MonoBehaviour
{
    public float tempsAffichage = 1.0f;
    private float tempsApparition;

    void Start()
    {
        tempsApparition = Time.time;
        Destroy(gameObject, tempsAffichage);
    }

    void OnMouseDown()
    {
        float score = Time.time - tempsApparition;

        // Utilisation de la méthode moderne 2026
        GenerateurCibles gen = Object.FindAnyObjectByType<GenerateurCibles>();
        if (gen != null)
        {
            gen.RecevoirScore(score);
        }
        Destroy(gameObject);
    }
}