using UnityEngine;
using UnityEngine.UI;

public class GyroManager : MonoBehaviour
{
    public Text debugText; // Glisse un objet UI Text ici dans l'Inspector

    void Start()
    {
        // 1. Vérifier si le téléphone possède un gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            // 2. Récupérer l'attitude (orientation spatiale)
            Quaternion attitude = Input.gyro.attitude;

            // 3. Affichage des données (X, Y, Z, W)
            debugText.text = "Orientation : " + attitude.ToString();
        }
        else
        {
            debugText.text = "Gyroscope non supporté sur cet appareil.";
        }
    }
}