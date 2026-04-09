using UnityEngine;

public class InclinaisonGyro : MonoBehaviour
{
    void Start()
    {
        // On vérifie si le téléphone possède bien un gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Debug.Log("Gyroscope activé avec succès !");
        }
        else
        {
            Debug.LogError("Ce téléphone ne possède pas de gyroscope.");
        }
    }

    void Update()
    {
        if (Input.gyro.enabled)
        {
            // Récupère la rotation
            Quaternion gyroRotation = Input.gyro.attitude;

            // Log de la rotation brute (X, Y, Z, W)
            // On utilise "Time.frameCount % 30 == 0" pour ne pas spammer la console 
            // (affiche le log environ toutes les demi-secondes)
            if (Time.frameCount % 30 == 0)
            {
                Debug.Log("Orientation Gyro : " + gyroRotation.ToString());
            }

            // Correction pour Unity
            Quaternion correction = new Quaternion(gyroRotation.x, gyroRotation.y, -gyroRotation.z, -gyroRotation.w);
            transform.localRotation = Quaternion.Euler(90f, 0f, 0f) * correction;
        }
    }
}