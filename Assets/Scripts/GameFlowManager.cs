using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    public TextMeshProUGUI bobText;
    public Button nextButton;
    public ObstacleGenerator obsGen;

    void Start()
    {
        // Au tout début, Bob annonce les obstacles
        ShowObstacles();
    }

    void ShowObstacles()
    {
        obsGen.GenerateObstacles();
        bobText.text = obsGen.sentenceObstacle;

        // On prépare le bouton pour l'étape suivante
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(ShowStartPositions);
    }

    void ShowStartPositions()
    {
        // On récupère les joueurs choisis dans le menu
        int count = PlayerPrefs.GetInt("PlayerCount", 0);
        string msg = "BOB : 'Bien. Maintenant, placez vos pions en bas des tours :'\n\n";

        for (int i = 0; i < count; i++)
        {
            string pName = PlayerPrefs.GetString("Player_" + i);
            // Random : Etage 1 ou Etage 16
            int start = (Random.value > 0.5f) ? 1 : 16;
            string tName = (start == 1) ? "Tour 1" : "Tour 2";

            msg += "- " + pName + " -> " + tName + " (Etage " + start + ")\n";
        }

        bobText.text = msg;

        // On prépare le bouton pour lancer la partie
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(FinalStart);
    }

    void FinalStart()
    {
        bobText.text = "BOB : 'Le jeu commence. Que la souffrance soit avec vous !'";
        nextButton.gameObject.SetActive(false); // On cache le bouton
    }
}