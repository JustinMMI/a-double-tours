using System.Collections;
using UnityEngine;

public class RoueWheel : MonoBehaviour
{
    [Header("Vitesse de rotation")]
    public float vitesseMin = 200f;
    public float vitesseMax = 400f;

    [Header("Décélération")]
    [Tooltip("Perte de vitesse par seconde après avoir appuyé sur STOP (degrés/s²).")]
    public float deceleration = 120f;

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
    private bool isDecelerating = false;

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

    /// <summary>
    /// Décélère progressivement la roue jusqu'à l'arrêt complet,
    /// puis appelle onArretee avec la section finale.
    /// </summary>
    public void Decelerate(System.Action<RoueSection> onArretee)
    {
        if (isDecelerating || isStopped) return;
        StartCoroutine(DecelerationRoutine(onArretee));
    }

    private IEnumerator DecelerationRoutine(System.Action<RoueSection> onArretee)
    {
        isDecelerating = true;

        while (vitesseActuelle > 0f)
        {
            vitesseActuelle -= deceleration * Time.deltaTime;
            vitesseActuelle = Mathf.Max(vitesseActuelle, 0f);
            yield return null;
        }

        isStopped = true;
        isDecelerating = false;

        onArretee?.Invoke(GetCurrentSection());
    }

    /// <summary>Remet la roue en rotation avec une vitesse et un angle de départ aléatoires.</summary>
    public void Restart()
    {
        StopAllCoroutines();
        isStopped = false;
        isDecelerating = false;
        vitesseActuelle = Random.Range(vitesseMin, vitesseMax);
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
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
