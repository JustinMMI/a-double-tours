using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    public TextMeshProUGUI bobText;
    public Button nextButton;
    public ObstacleGenerator obsGen;
    public Button duelButton;
    private bool canBobRollEvents = false;
    public string consequenceDuel = "Aucune conséquence définie";

    [Header("Consequences Settings")]
    public string[] randomConsequence;

    private string GetRandomConsequence()
    {
        if (randomConsequence == null || randomConsequence.Length == 0)
        {
            return "BOB : 'Aucun événement classique configuré pour le moment...'";
        }

        int randomIndex = Random.Range(0, randomConsequence.Length);

        if (randomConsequence.Length > 1)
        {
            while (randomIndex == lastEventIndex)
            {
                randomIndex = Random.Range(0, randomConsequence.Length);
            }
        }

        lastEventIndex = randomIndex;
        return randomConsequence[randomIndex];
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("FromDuel", 0) == 1)
        {
            consequenceDuel = GetRandomConsequence();
            canBobRollEvents = true;
            string winnerName = PlayerPrefs.GetString("DuelWinner");
            bobText.text = "BOB : 'Le minijeu est terminé ! Le sort en a décidé ainsi : le vainqueur est " + winnerName + " !' La consequence est : " + consequenceDuel + " !'";
            PlayerPrefs.SetInt("FromDuel", 0);
            nextButton.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("FromCacheCache", 0) == 1)
        {
            consequenceDuel = GetRandomConsequence();
            canBobRollEvents = true;
            string winnerName = PlayerPrefs.GetString("CacheCacheWinner");
            bobText.text = "BOB : 'Le Cache-Cache est terminé ! Le(s) vainqueur(s) : " + winnerName + " ! La conséquence est : " + consequenceDuel + " !'";
            PlayerPrefs.SetInt("FromCacheCache", 0);
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            canBobRollEvents = false;
            ShowObstacles();
            duelButton.gameObject.SetActive(false);
        }
    }

    void ShowObstacles()
    {
        obsGen.GenerateObstacles();
        bobText.text = obsGen.sentenceObstacle;

        List<string> startObstacles = obsGen.GetInitialObstacles();
        Debug.Log("[GameFlowManager] Bob annonce au début :\n" + bobText.text);
        if (startObstacles != null)
        {
            Debug.Log("[GameFlowManager] Obstacles initiaux reçus depuis ObstacleGenerator = " + string.Join(", ", startObstacles));
        }

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
        duelButton.gameObject.SetActive(true);
        canBobRollEvents = true;
    }

    [Header("Events Settings")]
    public string[] randomClassicEvent;

    /// <summary>
    /// Assign SceneAsset objects here in the Inspector. Scene names are synced automatically for runtime use.
    /// </summary>
    public List<Object> MiniGamePool;

    // Stores scene names extracted from MiniGamePool in the Editor, serialized for use in builds.
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

    public void OnBobClicked()
    {
        if (!canBobRollEvents)
        {
            return;
        }

        int eventRoll = Random.Range(0, 100); // 0..99

        if (eventRoll < 30)
        {
            // 30%
            bobText.text = "BOB : 'Aucun événement aléatoire cette fois-ci...'";
            return;
        }

        if (eventRoll < 55)
        {
            // 25%
            bobText.text = GenerateRandomObstacle();
            return;
        }

        if (eventRoll < 80)
        {
            // 25%
            bobText.text = GetRandomClassicEventOrFallback();
            return;
        }

        // 20%
        bobText.text = "BOB : 'Un mini-jeu est lancé !'";
        LoadRandomScene();
    }

    private string GetRandomClassicEventOrFallback()
    {
        if (randomClassicEvent == null || randomClassicEvent.Length == 0)
        {
            return "BOB : 'Aucun événement classique configuré pour le moment...'";
        }

        int randomIndex = Random.Range(0, randomClassicEvent.Length);

        if (randomClassicEvent.Length > 1)
        {
            while (randomIndex == lastEventIndex)
            {
                randomIndex = Random.Range(0, randomClassicEvent.Length);
            }
        }

        lastEventIndex = randomIndex;
        return randomClassicEvent[randomIndex];
    }

    private string GenerateRandomObstacle()
    {
        // Sécurité : vérifier que obsGen existe
        if (obsGen == null)
        {
            Debug.LogError("ObstacleGenerator n'est pas assigné !");
            return "BOB : 'Erreur système...'";
        }

        List<string> initialObstacles = obsGen.GetInitialObstacles();
        List<int> initialFloors = obsGen.GetInitialFloors();

        // Sécurité : vérifier que les listes ne sont pas nulles
        if (initialObstacles == null || initialFloors == null)
        {
            Debug.LogError("Les obstacles initiaux n'ont pas pu être récupérés !");
            return "BOB : 'Erreur système...'";
        }

        List<string> allObstacles = new List<string> { "Énergie", "Pierre", "Inondation", "Feu", "Tentacule de BOB", "Serpents" };

        // Exclure les obstacles déjà placés au début
        foreach (string initialObstacle in initialObstacles)
        {
            allObstacles.Remove(initialObstacle);
        }

        // Sécurité : vérifier qu'il reste des obstacles disponibles
        if (allObstacles.Count == 0)
        {
            Debug.LogWarning("Aucun obstacle disponible pour générer un événement !");
            return "BOB : 'Tous les dangers sont déjà placés...'";
        }

        // Sélectionner un obstacle aléatoire parmi les restants
        string selectedObstacle = allObstacles[Random.Range(0, allObstacles.Count)];

        // Exclure uniquement les positions exactes déjà occupées au début.
        HashSet<int> occupiedFloors = new HashSet<int>(initialFloors);

        // Étages valides
        List<int> possibleFloors = new List<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
        List<int> availableFloors = new List<int>();

        foreach (int floor in possibleFloors)
        {
            if (!occupiedFloors.Contains(floor))
            {
                availableFloors.Add(floor);
            }
        }

        // Sécurité : vérifier qu'il reste des étages disponibles
        if (availableFloors.Count == 0)
        {
            Debug.LogWarning("Aucun étage disponible pour placer l'obstacle !");
            return "BOB : 'Tous les étages sont occupés...'";
        }

        // Sélectionner un étage aléatoire
        int selectedFloor = availableFloors[Random.Range(0, availableFloors.Count)];
        string towerName = selectedFloor <= 15 ? "Tour 1" : "Tour 2";
        int floorDisplay = selectedFloor <= 15 ? selectedFloor : selectedFloor - 15;

        return "BOB : 'Un danger apparaît ! Placez " + selectedObstacle + " à " + towerName + ", étage " + floorDisplay + " !'";
    }

    private int lastEventIndex = -1;
    private int lastMiniGameIndex = -1;
    private string lastMiniGameSceneName = null;

    public void OnDuelClicked()
    {
        SceneManager.LoadScene("Gonflement ballon");
    }

    public void OnQuitClicked()
    {
        SceneManager.LoadScene("Main");
    }

}