using UnityEngine;

public class BilleDetection : MonoBehaviour
{
    public float limiteHauteur = -5f;

    void Update()
    {
        if (transform.position.y < limiteHauteur)
        {
            RecommencerBille("Tombé dans le vide !");
        }
    }

    private void OnTriggerEnter(Collider other)    {
        // CAS 1 : On touche un trou noir
        if (other.CompareTag("Trou"))
        {
            RecommencerBille("Perdu !");
        }

        // CAS 2 : On touche le trou de VICTOIRE
        if (other.CompareTag("Victoire"))
        {
            Gagner();
        }
    }

    void RecommencerBille(string message)
    {
        Debug.Log(message);
        transform.position = new Vector3(0, 2, 0);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Gagner()
    {
        // Pour l'instant on affiche juste un message, 
        // tu pourras plus tard charger le niveau suivant !
        Debug.Log("BRAVO ! TU AS GAGNÉ !");

        // On arrête la bille pour fêter ça
        GetComponent<Rigidbody>().isKinematic = true;
    }
}