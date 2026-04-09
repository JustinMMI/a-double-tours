using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DuelManager : MonoBehaviour
{
    public List<PionSelector> duelPions;
    public List<Object> MiniGamePool;

    [SerializeField] private List<string> miniGameSceneNames = new List<string>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        miniGameSceneNames.Clear();
        if (MiniGamePool == null) return;
        foreach (Object obj in MiniGamePool)
        {
            if (obj != null)
                miniGameSceneNames.Add(obj.name);
        }
    }
#endif

    private int lastMiniGameIndex = -1;
    private string lastMiniGameSceneName = null;

    public void LoadRandomScene()
    {
        if (miniGameSceneNames != null && miniGameSceneNames.Count > 0)
        {
            int randomIndex = Random.Range(0, miniGameSceneNames.Count);
            string selectedScene = miniGameSceneNames[randomIndex];

            if (miniGameSceneNames.Count > 1 && !string.IsNullOrEmpty(lastMiniGameSceneName))
            {
                int safety = 0;
                while (selectedScene == lastMiniGameSceneName && safety < 20)
                {
                    randomIndex = Random.Range(0, miniGameSceneNames.Count);
                    selectedScene = miniGameSceneNames[randomIndex];
                    safety++;
                }
            }

            lastMiniGameIndex = randomIndex;
            if (!string.IsNullOrEmpty(selectedScene))
            {
                lastMiniGameSceneName = selectedScene;
                SceneManager.LoadScene(selectedScene);
                Debug.Log("BOB charge la scène : " + selectedScene);
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

        // Sauvegarde les pions sélectionnés dans PlayerPrefs pour la scène mini-jeu
        PlayerPrefs.SetInt("DuelPlayerCount", selectedForDuel.Count);
        for (int i = 0; i < selectedForDuel.Count; i++)
        {
            PlayerPrefs.SetString("DuelPlayer_" + i, selectedForDuel[i]);
        }
        PlayerPrefs.SetInt("FromDuel", 1);
        PlayerPrefs.Save();

        LoadRandomScene();
    }
}