using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DuelManager : MonoBehaviour
{
    public List<PionSelector> duelPions;
    public List<Object> MiniGamePool;

    private int lastMiniGameIndex = -1;
    private string lastMiniGameSceneName = null;

    public void LoadRandomScene()
    {
        if (MiniGamePool != null && MiniGamePool.Count > 0)
        {
            int randomIndex = Random.Range(0, MiniGamePool.Count);
            Object selectedScene = MiniGamePool[randomIndex];

            if (MiniGamePool.Count > 1 && !string.IsNullOrEmpty(lastMiniGameSceneName))
            {
                int safety = 0;
                while (selectedScene != null && selectedScene.name == lastMiniGameSceneName && safety < 20)
                {
                    randomIndex = Random.Range(0, MiniGamePool.Count);
                    selectedScene = MiniGamePool[randomIndex];
                    safety++;
                }
            }

            lastMiniGameIndex = randomIndex;
            if (selectedScene != null)
            {
                lastMiniGameSceneName = selectedScene.name;
                SceneManager.LoadScene(selectedScene.name);
                Debug.Log("BOB charge la scène : " + selectedScene.name);
            }
        }
        else
        {
            Debug.LogWarning("La liste est vide, glisse des fichiers de scènes dedans !");
        }
    }

    public void OnConfirmDuel()
    {
        Debug.Log("1. Le bouton Start Duel a bien été cliqué !");

        List<string> selectedForDuel = new List<string>();

        foreach (PionSelector p in duelPions)
        {
            if (p.isSelected) selectedForDuel.Add(p.pionName);
        }

        Debug.Log("2. Nombre de pions sélectionnés : " + selectedForDuel.Count);

        if (selectedForDuel.Count != 2) return;

        int playersInGameCount = PlayerPrefs.GetInt("PlayerCount", 0);

        Debug.Log("3. Nombre de joueurs enregistrés au début de la partie : " + playersInGameCount);


        List<string> validPlayers = new List<string>();

        for (int i = 0; i < playersInGameCount; i++)
        {
            validPlayers.Add(PlayerPrefs.GetString("Player_" + i));
        }

        foreach (string name in selectedForDuel)
        {
            bool playerFound = false;

            foreach (string validName in validPlayers)
            {
                if (name.ToLower().Trim() == validName.ToLower().Trim())
                {
                    playerFound = true;
                    break;
                }
            }

            if (!playerFound)
            {
                Debug.Log("STOP : " + name + " n'est pas dans la partie ! (Vérifie l'orthographe)");
                return;
            }
        }

        Debug.Log("4. TOUT EST OK ! Lancement du duel...");

        string winner = selectedForDuel[Random.Range(0, 2)];
        PlayerPrefs.SetString("DuelWinner", winner);
        PlayerPrefs.SetInt("FromDuel", 1);

        SceneManager.LoadScene("GameScene");
    }
}