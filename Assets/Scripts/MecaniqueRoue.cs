using UnityEngine;
using UnityEngine.InputSystem;

public class MecaniqueRoue : MonoBehaviour
{
    [Header("Réglages")]
    public float friction = 0.98f;
    public float forceMin = 800f;
    public float forceMax = 1500f;

    private float vitesseActuelle = 0f;
    private bool estEnTrainDeTourner = false;

    void Update()
    {
        // LANCER (Espace)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !estEnTrainDeTourner)
        {
            vitesseActuelle = Random.Range(forceMin, forceMax);
            estEnTrainDeTourner = true;
            Debug.Log("Lancement 2D !");
        }

        // ROTATION
        if (vitesseActuelle > 0.1f)
        {
            // En 2D, on ne touche QUE l'axe Z.
            transform.Rotate(0, 0, vitesseActuelle * Time.deltaTime);
            vitesseActuelle *= friction;
        }
        else if (estEnTrainDeTourner)
        {
            ArreterProprement();
        }
    }

    void ArreterProprement()
    {
        vitesseActuelle = 0;
        estEnTrainDeTourner = false;

        // On récupère l'angle 2D (Z)
        float angleFinal = transform.eulerAngles.z;

        // On arrondit pour avoir un chiffre propre
        int resultatFinal = Mathf.RoundToInt(angleFinal);

        Debug.Log(" ARRÊT 2D ! Angle : " + resultatFinal + " degrés.");

        LogiqueGain(resultatFinal);
    }

    void LogiqueGain(int angle)
    {
        if (angle >= 0 && angle < 90) Debug.Log("ZONE SUD-EST");
        else if (angle >= 90 && angle < 180) Debug.Log("ZONE NORD-EST");
        else if (angle >= 180 && angle < 270) Debug.Log("ZONE NORD-OUEST");
        else Debug.Log("ZONE SUD-OUEST");
    }
}