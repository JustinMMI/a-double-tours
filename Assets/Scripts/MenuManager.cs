using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public List<PionSelector> allPions;
    public string GameScene;

    public void OnStartClicked()
    {
        List<string> selectedPions = new List<string>();

        // On vérifie qui est sélectionné
        foreach (PionSelector p in allPions)
        {
            if (p.isSelected)
            {
                selectedPions.Add(p.pionName);
            }
        }

        if (selectedPions.Count >= 2)
        {
            Debug.Log("Lancement avec " + selectedPions.Count + " joueurs.");

            PlayerPrefs.SetInt("PlayerCount", selectedPions.Count);
            for (int i = 0; i < selectedPions.Count; i++)
            {
                PlayerPrefs.SetString("Player_" + i, selectedPions[i]);
            }

            SceneManager.LoadScene(GameScene);
        }
        else
        {
            Debug.LogWarning("Il faut au moins 2 pions sélectionnés !");
        }
    }
}