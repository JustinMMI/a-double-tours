using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    public TextMeshProUGUI BobText;
    public Button NextButton;
    public LevelGenerator generator;

    void Start()
    {
        ShowObstaclesStep();
    }

    // Obstacles
    void ShowObstaclesStep()
    {
        generator.GenerateObstacles();
        BobText.text = generator.obstacleAnnouncement;

        NextButton.onClick.RemoveAllListeners();
        NextButton.onClick.AddListener(ShowStartPositionsStep);
    }

    // Positions
    void ShowStartPositionsStep()
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);
        string msg = "BOB : 'Maintenant, placez vos pions au point de départ !'\n\n";

        for (int i = 0; i < playerCount; i++)
        {
            string name = PlayerPrefs.GetString("Player_" + i);
            // Soit étage 1 (T1), soit étage 16 (T2)
            int startFloor = (Random.value > 0.5f) ? 1 : 16;
            string tower = (startFloor == 1) ? "Tour 1" : "Tour 2";

            msg += "- " + name + " -> " + tower + " (Etage " + startFloor + ")\n";
        }

        BobText.text = msg;

        NextButton.onClick.RemoveAllListeners();
        NextButton.onClick.AddListener(StartGame);
    }

    // Partie
    void StartGame()
    {
        BobText.text = "BOB : 'Tout est prêt. Que le premier joueur lance les dés !'";
        NextButton.gameObject.SetActive(false); // On peut cacher le bouton ou le changer en "Menu"
    }
}
