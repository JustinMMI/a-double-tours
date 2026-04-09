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

    private SpriteRenderer spriteRenderer;
    private float vitesseActuelle;
    private bool isStopped = false;
    private bool isDecelerating = false;
    private float startAngleOffset = 0f; 

    public bool IsStopped => isStopped;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Restart();
    }

    private void Update()
    {
        if (!isStopped)
        {
            float delta = vitesseActuelle * Time.deltaTime;
            transform.Rotate(delta, 0f, 0f);
        }

        if (spriteRenderer != null && sections.Length > 0)
            spriteRenderer.color = GetCurrentSection().couleur;
    }

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

        RoueSection finalSection = GetCurrentSection();
        if (spriteRenderer != null && finalSection != null)
            spriteRenderer.color = finalSection.couleur;

        onArretee?.Invoke(finalSection);
    }

    public void Restart()
    {
        StopAllCoroutines();
        isStopped = false;
        isDecelerating = false;
        vitesseActuelle = Random.Range(vitesseMin, vitesseMax);

        float initialAngle = Random.Range(0f, 360f);
        transform.eulerAngles = new Vector3(initialAngle, 0f, 0f);
        startAngleOffset = initialAngle;
    }

    public RoueSection GetCurrentSection()
    {
        if (sections == null || sections.Length == 0) return null;

        float sectionAngle = 360f / sections.Length;


        float rawAngle = transform.eulerAngles.x;
        float relativeAngle = (rawAngle - startAngleOffset + 360f) % 360f;

        int index = Mathf.FloorToInt(relativeAngle / sectionAngle) % sections.Length;
        return sections[index];
    }
}