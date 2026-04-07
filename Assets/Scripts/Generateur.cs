using UnityEngine;

public class GenerateurCibles : MonoBehaviour
{
    public GameObject prefabCible; // Le modèle de la cible
    public float intervalle = 0.1f; // Apparition toutes les 0.1s

    void Start()
    {
        // Appelle la fonction "Apparition" de façon répétée
        InvokeRepeating("Apparition", 0.1f, intervalle);
    }

    void Apparition()
    {
        // Choisit une position aléatoire sur l'écran
        Vector2 positionAleatoire = new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f));
        // Crée la cible
        Instantiate(prefabCible, positionAleatoire, Quaternion.identity);
    }
}