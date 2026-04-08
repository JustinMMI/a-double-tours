using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
    public int scoreun = 0;
    public int scoredeux = 0;
    public int scoreTrois = 0;
    public int scoreQuatre = 0;
    public int defineplayer = 1;

    void Start()
    {
        if (sequenceAffichée == null || saisieTexte == null || scoreTexte == null)
        {
            defineplayer = 1;
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

        sequenceAffichée.text = "Joueur " + defineplayer + " à ton tour ! Recopie le code.";
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

    public void EndGame()
    {
        CancelInvoke();
        StopAllCoroutines();

        var scores = new List<KeyValuePair<int, int>>
        {
            new KeyValuePair<int, int>(1, scoreun),
            new KeyValuePair<int, int>(2, scoredeux),
            new KeyValuePair<int, int>(3, scoreTrois),
            new KeyValuePair<int, int>(4, scoreQuatre)
        };
        scores.Sort((a, b) => b.Value.CompareTo(a.Value));

        string result = "Résultats :\n";
        for (int i = 0; i < scores.Count; i++)
        {
            result += (i + 1) + ". Joueur " + scores[i].Key + " : " + scores[i].Value + "\n";
        }

        sequenceAffichée.text = result;
        saisieActive = false;
        Time.timeScale = 0f;
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
            if (defineplayer == 1)
            {
                scoreun = score;
            }
            else if (defineplayer == 2)
            {
                scoredeux = score;
            }
            else if (defineplayer == 3)
            {
                scoreTrois = score;
            }
            else if (defineplayer == 4)
            {
                scoreQuatre = score;
                EndGame();
                return;
            }
            defineplayer = defineplayer + 1;
            scoreTexte.text = "Score : " + score;
            count = 2;
            score = 0;
            Invoke("GenererNouvelleSequence", 2.0f);
        }
    }
}
