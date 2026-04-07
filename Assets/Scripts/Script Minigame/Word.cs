using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Word : MonoBehaviour
{
    public TextMeshProUGUI[] textesBoutons; // Toujours glisser tes 4 textes ici une seule fois

    // On définit les familles de mots directement ici (plus besoin de l'inspecteur !)
    private string[][] familles = new string[][] {
        new string[] {"Lion", "Chat", "Chien", "Tigre", "Loup", "Ours"}, // Animaux
        new string[] {"Pomme", "Banane", "Fraise", "Poire", "Kiwi", "Cerise"}, // Fruits
        new string[] {"Voiture", "Avion", "Vélo", "Bateau", "Train", "Bus"}, // Transport
        new string[] {"Rouge", "Bleu", "Vert", "Violet", "Blanc", "Noir"}, // Couleur
        new string[] {"Soleil", "Pluie", "Neige", "Orage", "Nuage", "Vent"}, // Météo
        new string[] {"Table", "Chaise", "Lit", "Armoire", "Canapé", "Bureau"}, // Meubles
        new string[] {"Paris", "Londres", "Rome", "Tokyo", "Berlin", "Madrid"}, // Capitales
        new string[] {"Pizza", "Burger", "Sushis", "Pâtes", "Salade", "Tacos"}, // Plats
        new string[] {"Football", "Tennis", "Basket", "Judo", "Rugby", "Golf"}, // Sports
        new string[] {"Maman", "Papa", "Frère", "Sœur", "Oncle", "Tante"}, // Famille
        new string[] {"Couteau", "Fourchette", "Cuillère", "Assiette", "Bol", "Verre"}, // Vaisselle
        new string[] {"Mars", "Jupiter", "Vénus", "Saturne", "Terre", "Neptune"} // Planètes
    };

    private int indexCorrect;

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

        // 3. Préparer la liste finale de 4 mots
        List<string> motsDuJeu = new List<string>();

        // On prend 3 mots différents dans la famille principale
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
        motsDuJeu.Add(familleIntrus[Random.Range(0, familleIntrus.Length)]);

        // 4. Mélanger les 4 mots et retenir la position de l'intrus
        indexCorrect = Random.Range(0, 4);
        string motIntrus = motsDuJeu[3]; // L'intrus était le dernier ajouté
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
            Debug.Log("BRAVO ! C'était l'intrus.");
            NouvellePartie(); // Relance automatiquement un nouveau défi
        }
        else
        {
            Debug.Log("FAUX ! Essaie encore.");
        }
    }
}