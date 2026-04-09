using UnityEngine;

public class RoueWheel : MonoBehaviour
{
    [Header("Vitesse de rotation")]
    public float vitesseMin = 200f;
    public float vitesseMax = 400f;

    [Header("Sections (dans le sens horaire depuis le haut)")]
    public RoueSection[] sections = new RoueSection[]
    {
        new RoueSection { nom = "Jackpot",  points = 5, couleur = new Color(1f, 0.84f, 0f) },
        new RoueSection { nom = "Bien",     points = 3, couleur = new Color(0.56f, 0.93f, 0.56f) },
        new RoueSection { nom = "Moyen",    points = 2, couleur = new Color(0.53f, 0.81f, 0.98f) },
        new RoueSection { nom = "Jackpot",  points = 5, couleur = new Color(1f, 0.84f, 0f) },
        new RoueSection { nom = "Raté",     points = 0, couleur = new Color(1f, 0.42f, 0.42f) },
        new RoueSection { nom = "Bien",     points = 3, couleur = new Color(0.56f, 0.93f, 0.56f) },
    };

    private float vitesseActuelle;
    private bool isStopped = false;

    public bool IsStopped => isStopped;

    private void Start()
    {
        vitesseActuelle = Random.Range(vitesseMin, vitesseMax);
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
    }

    private void Update()
    {
        if (isStopped) return;
        transform.Rotate(0f, 0f, -vitesseActuelle * Time.deltaTime);
    }

    /// <summary>Arrête la roue et retourne la section en face de l'indicateur.</summary>
    public RoueSection Stop()
    {
        isStopped = true;
        return GetCurrentSection();
    }

    /// <summary>Remet la roue en rotation avec une vitesse aléatoire.</summary>
    public void Restart()
    {
        isStopped = false;
        vitesseActuelle = Random.Range(vitesseMin, vitesseMax);
    }

    /// <summary>Retourne la section actuellement pointée par l'indicateur (haut = 0°).</summary>
    public RoueSection GetCurrentSection()
    {
        if (sections == null || sections.Length == 0) return null;

        float sectionAngle = 360f / sections.Length;
        float eulerZ = transform.eulerAngles.z;
        float normalizedAngle = (((-eulerZ) % 360f) + 360f) % 360f;
        int index = Mathf.FloorToInt(normalizedAngle / sectionAngle) % sections.Length;

        return sections[index];
    }
}
