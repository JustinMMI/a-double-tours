using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem; // Requis pour le nouveau système

public class Mining : MonoBehaviour
{
    [Header("Réglages de Détection")]
    [Tooltip("Force minimale du mouvement (1.5 - 3.0 est une bonne base)")]
    public float threshold = 2.0f;
    [Tooltip("Temps en secondes pour accumuler l'énergie du geste")]
    public float detectionWindow = 0.5f;

    [Header("Cible et Portée")]
    public LayerMask wallLayer;
    public float reachDistance = 3.0f;

    private List<float> accelerationHistory = new List<float>();
    private float timer = 0f;

    void Start()
    {
        // TRÈS IMPORTANT : Activer l'accéléromètre sur le nouveau système
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("Accéléromètre activé");
        }
        else
        {
            Debug.LogWarning("Aucun accéléromètre détecté sur cet appareil.");
        }
    }

    void Update()
    {
        // 1. GESTION DE L'ACCÉLÉRATION
        Vector3 accelValue = Vector3.zero;
        if (Accelerometer.current != null)
        {
            accelValue = Accelerometer.current.acceleration.ReadValue();
        }

        // On calcule la force du mouvement (magnitude)
        // La valeur est souvent autour de 1.0 au repos (gravité), on soustrait donc 1
        float currentForce = Mathf.Max(0, accelValue.magnitude - 1.0f);

        // 2. CALCUL SUR L'INTERVALLE DE TEMPS
        timer += Time.deltaTime;
        accelerationHistory.Add(currentForce);

        // On nettoie les anciennes valeurs pour rester dans la fenêtre de temps
        if (timer > detectionWindow)
        {
            if (accelerationHistory.Count > 0)
            {
                accelerationHistory.RemoveAt(0);
            }
        }

        // Calcul de la moyenne cumulée
        float averageForce = 0;
        if (accelerationHistory.Count > 0)
        {
            foreach (float f in accelerationHistory) averageForce += f;
            averageForce /= accelerationHistory.Count;
        }

        // 3. DÉTECTION DU COUP (MOBILE OU PC)
        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (averageForce > threshold || spacePressed)
        {
            TryBreakWall(averageForce > 0 ? averageForce : 10f); // 10f est une force par défaut pour le test PC
            accelerationHistory.Clear(); // Évite les déclenchements multiples
        }

        // Visualisation du rayon dans l'éditeur
        Debug.DrawRay(transform.position, transform.forward * reachDistance, Color.cyan);
    }

    void TryBreakWall(float power)
    {
        RaycastHit hit;
        // Lance un rayon depuis le centre de l'écran ou l'objet vers l'avant
        if (Physics.Raycast(transform.position, transform.forward, out hit, reachDistance, wallLayer))
        {
            Debug.Log("<color=green>MUR TOUCHÉ !</color> Force appliquée : " + power);

            // Option simple : destruction
            Destroy(hit.collider.gameObject);

            // Vibration
#if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
#endif
        }
    }
}