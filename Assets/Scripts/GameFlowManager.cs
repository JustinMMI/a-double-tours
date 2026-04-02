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
        ShowObstacles();
    }

    void ShowObstacles()
    {
        obsGen.GenerateObstacles();
        bobText.text = obsGen.sentenceObstacle;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(ShowStartPositions);
    }

    void ShowStartPositions()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);
        string msg = "BOB : 'Bien. Maintenant, placez vos pions en bas des tours :'\n\n";

        for (int i = 0; i < count; i++)
        {
            string pName = PlayerPrefs.GetString("Player_" + i);
            // Random : Etage 1 ou Etage 16
            int start = (Random.value > 0.5f) ? 1 : 16;
            string tName = (start == 1) ? "Tour 1" : "Tour 2";

            msg += "- " + pName + " -> " + tName + "\n";
        }

        bobText.text = msg;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(FinalStart);
    }

    void FinalStart()
    {
        bobText.text = "BOB : 'Le jeu commence. Que la souffrance soit avec vous !'";
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(End);
    }

    void End()
    {
        bobText.text = "";
        nextButton.gameObject.SetActive(false);
    }

    [Header("Events Settings")]
    public string[] randomEvents = {
    "BOB : 'Un séisme secoue la tour ! Tout le monde recule d'une case.'",
    "BOB : 'L'obscurité envahit la tour, les joueurs ne se voient plus. Aucun duel ne peut être initié au prochain tour.'",
    "BOB : 'C'est pas un peu long là ? Tout le monde monte d'un étage.'",
    "BOB : 'Ouverture d'un portail temporel, tout le monde switch de tour !'",
    "BOB : 'Le joueur en tête a trop d'avance, il est frappé par une malédiction et redescend de 3 étages.'",
    "BOB : 'Joueur en queue de peloton, une force mystérieuse te propulse vers le haut, avance de 2 étages !'",
};

    public void OnBobClicked()
    {
        int randomIndex = Random.Range(0, randomEvents.Length);
        bobText.text = randomEvents[randomIndex];
    }
}