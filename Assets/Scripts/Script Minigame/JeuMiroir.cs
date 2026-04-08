using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class JeuMiroir : MonoBehaviour
{
    [Header("Configuration UI")]
    public TMP_Text sequenceAffichée;
    public TMP_Text saisieTexte;
    public TMP_Text scoreTexte;

    [Header("Réglages du Jeu")]
    public float tempsAffichage = 2.0f;

    private string sequenceActuelle = "";
    private string saisieActuelle = "";
    private int score = 0;
    private bool saisieActive = false;

    void Start()
    {
        if (sequenceAffichée == null || saisieTexte == null || scoreTexte == null)
        {
            Debug.LogError("[JeuMiroir] Un ou plusieurs champs UI ne sont pas assignés dans l'Inspector.", this);
            return;
        }

        saisieActuelle = "";
        RefreshSaisie();
        scoreTexte.text = "Score : 0";
        GenererNouvelleSequence();
    }

    public int count = 2;
    public void GenererNouvelleSequence()
    {
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        sequenceActuelle = "";
        for (int i = 0; i < count; i++)
        {
            sequenceActuelle += caracteres[Random.Range(0, caracteres.Length)];
        }

        count = count + 1;
        StopAllCoroutines();
        StartCoroutine(AfficherPuisCacher());
    }

    IEnumerator AfficherPuisCacher()
    {
        sequenceAffichée.text = "Bob dit : " + sequenceActuelle;
        saisieActuelle = "";
        RefreshSaisie();
        saisieActive = false;

        yield return new WaitForSeconds(tempsAffichage);

        sequenceAffichée.text = "À ton tour ! Recopie le code.";
        saisieActive = true;
    }

    /// <summary>Appelée par chaque bouton lettre du clavier.</summary>
    public void AppuyerLettre(string lettre)
    {
        if (!saisieActive) return;
        if (saisieActuelle.Length >= sequenceActuelle.Length) return;
        saisieActuelle += lettre;
        RefreshSaisie();

        if (saisieActuelle.Length == sequenceActuelle.Length)
            VerifierReponse();
    }

    /// <summary>Efface la dernière lettre saisie.</summary>
    public void Effacer()
    {
        if (!saisieActive || saisieActuelle.Length == 0) return;
        saisieActuelle = saisieActuelle.Substring(0, saisieActuelle.Length - 1);
        RefreshSaisie();
    }

    /// <summary>Valide manuellement la saisie courante si elle est complète.</summary>
    public void Valider()
    {
        if (!saisieActive || saisieActuelle.Length == 0) return;
        VerifierReponse();
    }

    private void RefreshSaisie()
    {
        saisieTexte.text = saisieActuelle;
    }

    public void VerifierReponse()
    {
        if (saisieActuelle.ToUpper() == sequenceActuelle)
        {
            score++;
            scoreTexte.text = "Score : " + score;
            GenererNouvelleSequence();
        }
        else
        {
            saisieActive = false;
            sequenceAffichée.text = "PERDU ! C'était : " + sequenceActuelle;
            score = 0;
            scoreTexte.text = "Score : 0";
            Invoke("GenererNouvelleSequence", 2.0f);
        }
    }
}
