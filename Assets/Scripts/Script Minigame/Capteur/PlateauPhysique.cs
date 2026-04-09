using UnityEngine; // On utilise les outils de base d'Unity.

public class PlateauPhysique : MonoBehaviour
{
    // C'est la force du mouvement (la sensibilité).
    // On met 25 pour que la table réagisse bien à tes mains.
    public float forceInclinaison = 25f;

    // Cette partie s'exécute 1 seule fois au début.
    void Start()
    {
        // On allume les capteurs de mouvement du téléphone.
        Input.gyro.enabled = true;
    }

    // Cette partie se répète tout le temps (60 fois par seconde).
    void Update()
    {
        // 1. On demande au téléphone : "Comment tu es penché ?"
        // Le téléphone répond avec des chiffres (X, Y, Z).
        Vector3 inclinaison = Input.acceleration;

        // 2. On calcule l'angle pour aller en avant ou en arrière.
        // On multiplie par 25 pour que le mouvement soit visible.
        float rotX = inclinaison.y * forceInclinaison;

        // 3. On calcule l'angle pour aller à gauche ou à droite.
        // On met un "-" pour que la table penche du même côté que tes mains.
        float rotZ = -inclinaison.x * forceInclinaison;

        // 4. On donne l'ordre final à la table de pivoter.
        // On utilise "Quaternion.Euler" pour transformer nos chiffres en rotation.
        // On laisse le 0 au milieu car la table ne doit pas tourner sur elle-même.
        transform.rotation = Quaternion.Euler(rotX, 0, rotZ);
    }
}