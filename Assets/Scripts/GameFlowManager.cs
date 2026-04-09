using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    private enum BobEventType
    {
        None,
        Obstacle,
        Classic,
        MiniGame
    }

    private enum ClassicEventStyle
    {
        Any,
        Leader,
        Last,
        AllPlayers,
        DuelPlayers,
        SinglePlayer
    }

    public TextMeshProUGUI bobText;
    public Button nextButton;
    public ObstacleGenerator obsGen;
    public Button duelButton;
    public Button rerollButton;
    public Button okButton;
    private bool canBobRollEvents = false;
    public string consequenceDuel = "Aucune conséquence définie";

    [Header("Consequences Settings")]
    public string[] randomConsequence;
    public bool consequenceSwitch = false;

    private int lastConsequenceIndex = -1;
    private int lastMiniGameIndex = -1;
    private int lastEventRoll = -1;
    private int lastClassicEventIndex = -1;
    private int lastObstacleIndex = -1;
    private int lastObstacleFloor = -1;
    private BobEventType lastBobEventType = BobEventType.None;
    private const float bobClickCooldown = 2f;
    private float lastBobClickTime = -10f;

    public void GetRandomConsequence()
    {
        if (randomConsequence == null || randomConsequence.Length == 0)
        {
            consequenceDuel = "BOB : 'Aucun événement classique configuré pour le moment...'";
            return;
        }

        int randomIndex = Random.Range(0, randomConsequence.Length);
        if (randomConsequence.Length > 1)
        {
            while (randomIndex == lastConsequenceIndex)
            {
                randomIndex = Random.Range(0, randomConsequence.Length);
            }
        }

        lastConsequenceIndex = randomIndex;
        consequenceDuel = randomConsequence[randomIndex];
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("FromDuel", 0) == 1)
        {
            obsGen.LoadObstaclesFromPrefs();
            GetRandomConsequence();
            canBobRollEvents = true;
            string winnerName = PlayerPrefs.GetString("DuelWinner");
            bobText.text = "BOB : 'Le mini jeu est terminé ! Le sort en a décidé ainsi : le vainqueur est " + winnerName + " !' La consequence est : " + consequenceDuel + " !'";
            PlayerPrefs.SetInt("FromDuel", 0);
            nextButton.gameObject.SetActive(false);
            rerollButton.gameObject.SetActive(true);
            okButton.gameObject.SetActive(true);
            consequenceSwitch = true;
        }
        else if (PlayerPrefs.GetInt("FromCacheCache", 0) == 1)
        {
            obsGen.LoadObstaclesFromPrefs();
            GetRandomConsequence();
            canBobRollEvents = true;
            string winnerName = PlayerPrefs.GetString("CacheCacheWinner");
            bobText.text = "BOB : 'Le Cache-Cache est terminé ! Le(s) vainqueur(s) : " + winnerName + " ! La conséquence est : " + consequenceDuel + " !'";
            PlayerPrefs.SetInt("FromCacheCache", 0);
            nextButton.gameObject.SetActive(false);
            rerollButton.gameObject.SetActive(true);
            okButton.gameObject.SetActive(true);
            consequenceSwitch = true;
        }
        else if (PlayerPrefs.GetInt("fromRerollKey", 0) == 1)
        {
            PlayerPrefs.SetInt("fromRerollKey", 0);
            LoadRandomScene();
        }
        else
        {
            canBobRollEvents = false;
            ShowObstacles();
            duelButton.gameObject.SetActive(false);
            rerollButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(false);
        }
    }

    public void RerollConsequence()
    {
        if (consequenceSwitch == true)        {
            GetRandomConsequence();
            bobText.text = "BOB : 'La nouvelle conséquence est : " + consequenceDuel + " !'";
        }
        else
        {
            RerollSameStyleEvent();
        }
    }

    private void RerollSameStyleEvent()
    {
        if (lastBobEventType == BobEventType.Obstacle)
        {
            bobText.text = GenerateRandomObstacle();
            return;
        }

        if (lastBobEventType == BobEventType.Classic)
        {
            bobText.text = GetRandomClassicEventSameStyleOrFallback();
            return;
        }

        int forcedRoll = Random.Range(0, 80);
        if (forcedRoll < 30)
        {
            bobText.text = "BOB : 'Aucun événement aléatoire cette fois-ci...'";
            return;
        }

        if (forcedRoll < 55)
        {
            lastBobEventType = BobEventType.Obstacle;
            bobText.text = GenerateRandomObstacle();
            return;
        }

        lastBobEventType = BobEventType.Classic;
        bobText.text = GetRandomClassicEventOrFallback();
    }

    public void OkConsequence()
    {
        bobText.text = "";
        rerollButton.gameObject.SetActive(false);
        okButton.gameObject.SetActive(false);
        consequenceSwitch = false;
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
        List<string> playerNames = new List<string>();

        for (int i = 0; i < count; i++)
        {
            string pName = PlayerPrefs.GetString("Player_" + i);
            playerNames.Add(pName);
            int start = (Random.value > 0.5f) ? 1 : 16;
            string tName = (start == 1) ? "Tour 1" : "Tour 2";

            msg += "- " + pName + " -> " + tName + "\n";
        }

        if (playerNames.Count > 0)
        {
            string firstPlayer = playerNames[Random.Range(0, playerNames.Count)];
            msg += "\nBOB : '" + firstPlayer + " commence la partie !'";
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

    public void LoadRandomScene()
    {
        if (miniGameSceneNames != null && miniGameSceneNames.Count > 0)
        {
            int randomIndex = Random.Range(0, miniGameSceneNames.Count);
            if (miniGameSceneNames.Count > 1)
            {
                while (randomIndex == lastMiniGameIndex)
                {
                    randomIndex = Random.Range(0, miniGameSceneNames.Count);
                }
            }

            lastMiniGameIndex = randomIndex;
            string selectedScene = miniGameSceneNames[randomIndex];

            if (!string.IsNullOrEmpty(selectedScene))
            {
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
        if (Time.time - lastBobClickTime < bobClickCooldown)
        {
            return;
        }

        if (!canBobRollEvents)
        {
            return;
        }

        lastBobClickTime = Time.time;

        int eventRoll = Random.Range(0, 100);
        while (eventRoll == lastEventRoll)
        {
            eventRoll = Random.Range(0, 100);
        }
        lastEventRoll = eventRoll;

        rerollButton.gameObject.SetActive(true);
        okButton.gameObject.SetActive(true);

        if (eventRoll < 30)
        {
            bobText.text = "BOB : 'Aucun événement aléatoire cette fois-ci...'";
            return;
        }

        if (eventRoll < 55)
        {
            lastBobEventType = BobEventType.Obstacle;
            bobText.text = GenerateRandomObstacle();
            return;
        }

        if (eventRoll < 80)
        {
            lastBobEventType = BobEventType.Classic;
            bobText.text = GetRandomClassicEventOrFallback();
            return;
        }

        lastBobEventType = BobEventType.MiniGame;
        bobText.text = "BOB : 'Un mini-jeu est lancé !'";
        LoadRandomScene();
    }

    private string GetRandomClassicEventSameStyleOrFallback()
    {
        if (randomClassicEvent == null || randomClassicEvent.Length == 0)
        {
            return "BOB : 'Aucun événement classique configuré pour le moment...'";
        }

        if (lastClassicEventIndex < 0 || lastClassicEventIndex >= randomClassicEvent.Length)
        {
            return GetRandomClassicEventOrFallback();
        }

        ClassicEventStyle wantedStyle = DetectClassicEventStyle(randomClassicEvent[lastClassicEventIndex]);
        List<int> candidates = new List<int>();

        for (int i = 0; i < randomClassicEvent.Length; i++)
        {
            if (i == lastClassicEventIndex) continue;

            ClassicEventStyle currentStyle = DetectClassicEventStyle(randomClassicEvent[i]);
            if (currentStyle == wantedStyle)
            {
                candidates.Add(i);
            }
        }

        if (candidates.Count == 0)
        {
            return GetRandomClassicEventOrFallback();
        }

        int picked = candidates[Random.Range(0, candidates.Count)];
        lastClassicEventIndex = picked;
        return randomClassicEvent[picked];
    }

    private ClassicEventStyle DetectClassicEventStyle(string evt)
    {
        if (string.IsNullOrEmpty(evt)) return ClassicEventStyle.Any;

        string t = evt.ToLower();

        // petite detection simple, on touche pas les listes
        if (t.Contains("tete") || t.Contains("1er") || t.Contains("premier")) return ClassicEventStyle.Leader;
        if (t.Contains("dernier") || t.Contains("retard")) return ClassicEventStyle.Last;
        if (t.Contains("tous") || t.Contains("tout le monde")) return ClassicEventStyle.AllPlayers;
        if (t.Contains("duel") || t.Contains("combat") || t.Contains("2 joueurs")) return ClassicEventStyle.DuelPlayers;
        if (t.Contains("un joueur") || t.Contains("joueur choisi") || t.Contains("au hasard")) return ClassicEventStyle.SinglePlayer;

        return ClassicEventStyle.Any;
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
            while (randomIndex == lastClassicEventIndex)
            {
                randomIndex = Random.Range(0, randomClassicEvent.Length);
            }
        }

        lastClassicEventIndex = randomIndex;
        return randomClassicEvent[randomIndex];
    }

    private string GenerateRandomObstacle()
    {
        if (obsGen == null)
        {
            Debug.LogError("ObstacleGenerator n'est pas assigné !");
            return "BOB : 'Erreur système...'";
        }

        List<string> initialObstacles = obsGen.GetInitialObstacles();
        List<int> initialFloors = obsGen.GetInitialFloors();

        if (initialObstacles == null || initialFloors == null)
        {
            Debug.LogError("Les obstacles initiaux n'ont pas pu être récupérés !");
            return "BOB : 'Erreur système... Initial'";
        }

        List<string> allObstacles = new List<string> { "Surcharge", "Eboulement", "Inondation", "Incendie", "Tentacule de BOB", "Serpents" };

        foreach (string initialObstacle in initialObstacles)
        {
            allObstacles.Remove(initialObstacle);
        }

        if (allObstacles.Count == 0)
        {
            Debug.LogWarning("Aucun obstacle disponible pour générer un événement !");
            return "BOB : 'Tous les dangers sont déjà placés...'";
        }

        int obstacleIndex = Random.Range(0, allObstacles.Count);
        if (allObstacles.Count > 1)
        {
            while (obstacleIndex == lastObstacleIndex)
            {
                obstacleIndex = Random.Range(0, allObstacles.Count);
            }
        }

        lastObstacleIndex = obstacleIndex;
        string selectedObstacle = allObstacles[obstacleIndex];

        HashSet<int> occupiedFloors = new HashSet<int>(initialFloors);

        // exclusion etage au dessus
        foreach (int initialFloor in initialFloors)
        {
            int floorAbove = initialFloor + 1;
            bool sameTower1 = initialFloor <= 15 && floorAbove <= 15;
            bool sameTower2 = initialFloor >= 21 && floorAbove <= 30;

            if (sameTower1 || sameTower2)
            {
                occupiedFloors.Add(floorAbove);
            }
        }

        List<int> possibleFloors = new List<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
        List<int> availableFloors = new List<int>();

        foreach (int floor in possibleFloors)
        {
            if (!occupiedFloors.Contains(floor))
            {
                availableFloors.Add(floor);
            }
        }

        if (availableFloors.Count == 0)
        {
            Debug.LogWarning("Aucun étage disponible pour placer l'obstacle !");
            return "BOB : 'Tous les étages sont occupés...'";
        }

        int selectedFloor = availableFloors[Random.Range(0, availableFloors.Count)];
        if (availableFloors.Count > 1)
        {
            while (selectedFloor == lastObstacleFloor)
            {
                selectedFloor = availableFloors[Random.Range(0, availableFloors.Count)];
            }
        }

        lastObstacleFloor = selectedFloor;
        string towerName = selectedFloor <= 15 ? "Tour 1" : "Tour 2";
        int floorDisplay = selectedFloor <= 15 ? selectedFloor : selectedFloor - 15;

        return "BOB : 'Un danger apparaît ! Placez " + selectedObstacle + " à " + towerName + ", étage " + floorDisplay + " !'";
    }

    public void OnDuelClicked()
    {
        SceneManager.LoadScene("DuelMenu");
    }

    public void OnQuitClicked()
    {
        SceneManager.LoadScene("Main");
    }
}