using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Word : MonoBehaviour
{
    public TextMeshProUGUI[] textesBoutons; // Toujours glisser tes 4 textes ici une seule fois

    // On d�finit les familles de mots directement ici (plus besoin de l'inspecteur !)
    private string[][] familles = new string[][] {
        new string[] {"Lion", "Chat", "Chien", "Tigre", "Loup", "Ours"}, // Animaux
        new string[] {"Pomme", "Banane", "Fraise", "Poire", "Kiwi", "Cerise"}, // Fruits
        new string[] {"Voiture", "Avion", "V�lo", "Bateau", "Train", "Bus"}, // Transport
        new string[] {"Rouge", "Bleu", "Vert", "Violet", "Blanc", "Noir"}, // Couleur
        new string[] {"Soleil", "Pluie", "Neige", "Orage", "Nuage", "Vent"}, // M�t�o
        new string[] {"Table", "Chaise", "Lit", "Armoire", "Canap�", "Bureau"}, // Meubles
        new string[] {"Paris", "Londres", "Rome", "Tokyo", "Berlin", "Madrid"}, // Capitales
        new string[] {"Pizza", "Burger", "Sushis", "P�tes", "Salade", "Tacos"}, // Plats
        new string[] {"Football", "Tennis", "Basket", "Judo", "Rugby", "Golf"}, // Sports
        new string[] {"Maman", "Papa", "Fr�re", "S�ur", "Oncle", "Tante"}, // Famille
        new string[] {"Couteau", "Fourchette", "Cuill�re", "Assiette", "Bol", "Verre"}, // Vaisselle
        new string[] {"Mars", "Jupiter", "V�nus", "Saturne", "Terre", "Neptune"} // Plan�tes
    };

    private int indexCorrect;
    private int lastIntrusWordIndex = -1;

    void Start()
    {
        NouvellePartie();
    }

    public void NouvellePartie()
    {
        // 1. Choisir une famille au hasard pour les 3 mots corrects
        int indexFamillePrincipale = Random.Range(0, familles.Length);
        string[] familleChoisie = familles[indexFamillePrincipale];

        // 2. Choisir une AUTRE famille au hasard pour l'intrus
        int indexFamilleIntrus;
        do
        {
            indexFamilleIntrus = Random.Range(0, familles.Length);
        } while (indexFamilleIntrus == indexFamillePrincipale);

        string[] familleIntrus = familles[indexFamilleIntrus];

        // 3. Pr�parer la liste finale de 4 mots
        List<string> motsDuJeu = new List<string>();

        // On prend 3 mots diff�rents dans la famille principale
        List<int> indicesUtilises = new List<int>();
        while (motsDuJeu.Count < 3)
        {
            int r = Random.Range(0, familleChoisie.Length);
            if (!indicesUtilises.Contains(r))
            {
                motsDuJeu.Add(familleChoisie[r]);
                indicesUtilises.Add(r);
            }
        }

        // On ajoute 1 mot au hasard de la famille intrus
        int intrusIndex = Random.Range(0, familleIntrus.Length);
        if (familleIntrus.Length > 1)
        {
            while (intrusIndex == lastIntrusWordIndex)
            {
                intrusIndex = Random.Range(0, familleIntrus.Length);
            }
        }

        lastIntrusWordIndex = intrusIndex;
        motsDuJeu.Add(familleIntrus[intrusIndex]);

        // 4. M�langer les 4 mots et retenir la position de l'intrus
        indexCorrect = Random.Range(0, 4);
        string motIntrus = motsDuJeu[3]; // L'intrus �tait le dernier ajout�
        motsDuJeu[3] = motsDuJeu[indexCorrect];
        motsDuJeu[indexCorrect] = motIntrus;

        // 5. Affichage sur les boutons
        for (int i = 0; i < textesBoutons.Length; i++)
        {
            textesBoutons[i].text = motsDuJeu[i];
        }
    }

    public void VerifierChoix(int indexClique)
    {
        if (indexClique == indexCorrect)
        {
            Debug.Log("BRAVO ! C'�tait l'intrus.");
            NouvellePartie(); // Relance automatiquement un nouveau d�fi
        }
        else
        {
            Debug.Log("FAUX ! Essaie encore.");
        }
    }
}