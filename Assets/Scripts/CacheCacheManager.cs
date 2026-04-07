using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CacheCacheManager : MonoBehaviour
{
    public enum GamePhase { HiderSelection, SeekerSelection, Result }
    public GamePhase CurrentPhase { get; private set; } = GamePhase.HiderSelection;

    private List<string> hiderNames = new List<string>();
    private string seekerName;
    private int currentHiderIndex = 0;

    private Dictionary<int, List<string>> bushOccupancy = new Dictionary<int, List<string>>();
    private Dictionary<string, int> hiderChoices = new Dictionary<string, int>();
    private int seekerBushChoice = -1;

    private List<string> activehiders = new List<string>();
    private HashSet<int> disabledBushes = new HashSet<int>();

    // Nombre total de ratés du chasseur (ne se remet jamais à zéro)
    private int totalMisses = 0;
    private const int MaxMisses = 2;

    [Header("UI")]
    [SerializeField] private CacheCacheUI ui;

    private const int BushCount = 5;
    private const string ReturnSceneName = "GameScene";
    private const string SeekerIndexKey = "CacheCache_SeekerIndex";
    private const float ContinueDelay = 5f;


    private void Start()
    {
        InitializePlayers();
        InitializeBushOccupancy();
        BeginHiderPhase();
    }


    private void InitializePlayers()
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);

        if (playerCount == 0)
        {
            hiderNames = new List<string> { "Alice", "Bob", "Charlie" };
            seekerName = "David";
            Debug.Log("Il n'y a pas de vrai joueur, j'utilise donc des faux joueurs.");
            activehiders = new List<string>(hiderNames);
            return;
        }

        int seekerIndex = PlayerPrefs.GetInt(SeekerIndexKey, playerCount - 1);

        for (int i = 0; i < playerCount; i++)
        {
            string name = PlayerPrefs.GetString("Player_" + i, "Joueur " + (i + 1));
            if (i == seekerIndex)
                seekerName = name;
            else
                hiderNames.Add(name);
        }

        if (string.IsNullOrEmpty(seekerName) && hiderNames.Count > 0)
        {
            seekerName = hiderNames[hiderNames.Count - 1];
            hiderNames.RemoveAt(hiderNames.Count - 1);
        }

        activehiders = new List<string>(hiderNames);
        Debug.Log($"[CacheCache] Chasseur : {seekerName} | Cacheurs : {string.Join(", ", hiderNames)}");
    }


    private void InitializeBushOccupancy()
    {
        for (int i = 0; i < BushCount; i++)
            bushOccupancy[i] = new List<string>();
    }


    private void BeginHiderPhase()
    {
        CurrentPhase = GamePhase.HiderSelection;
        currentHiderIndex = 0;
        PromptCurrentHider();
    }

    private void PromptCurrentHider()
    {
        if (currentHiderIndex >= hiderNames.Count)
        {
            BeginSeekerPhase();
            return;
        }

        string currentHider = hiderNames[currentHiderIndex];
        ui.ShowHiderSelection(currentHider, currentHiderIndex, hiderNames.Count, disabledBushes);
    }

    public void OnHiderChoseBush(int bushIndex)
    {
        if (CurrentPhase != GamePhase.HiderSelection) return;

        string currentHider = hiderNames[currentHiderIndex];
        hiderChoices[currentHider] = bushIndex;
        bushOccupancy[bushIndex].Add(currentHider);

        Debug.Log($"[CacheCache] {currentHider} se cache dans le buisson {bushIndex + 1}");

        currentHiderIndex++;
        PromptCurrentHider();
    }


    private void BeginSeekerPhase()
    {
        CurrentPhase = GamePhase.SeekerSelection;
        int missesLeft = MaxMisses - totalMisses;
        ui.ShowSeekerSelection(seekerName, disabledBushes, missesLeft);
    }

    public void OnSeekerChoseBush(int bushIndex)
    {
        if (CurrentPhase != GamePhase.SeekerSelection) return;

        seekerBushChoice = bushIndex;
        Debug.Log($"[CacheCache] {seekerName} fouille le buisson {bushIndex + 1}");
        ResolveRound();
    }


    private void ResolveRound()
    {
        CurrentPhase = GamePhase.Result;

        List<string> caught = new List<string>(bushOccupancy[seekerBushChoice]);

        if (caught.Count > 0)
        {
            foreach (string eliminated in caught)
                activehiders.Remove(eliminated);
        }
        else
        {
            totalMisses++;
        }

        disabledBushes.Add(seekerBushChoice);

        List<string> escaped = new List<string>(activehiders);

        bool seekerWins  = activehiders.Count == 0;
        bool seekerLoses = totalMisses >= MaxMisses;
        bool canContinue = !seekerWins && !seekerLoses && HasAvailableBush();

        Debug.Log($"[CacheCache] Ratés totaux : {totalMisses}/{MaxMisses}");

        ui.ShowResult(
            seekerName,
            seekerBushChoice,
            caught,
            escaped,
            hiderChoices,
            BushCount,
            disabledBushes,
            seekerWins,
            seekerLoses,
            canContinue
        );

        if (canContinue)
            StartCoroutine(AutoContinueAfterDelay());
    }

    private IEnumerator AutoContinueAfterDelay()
    {
        yield return new WaitForSeconds(ContinueDelay);
        seekerBushChoice = -1;
        BeginSeekerPhase();
    }

    private bool HasAvailableBush()
    {
        foreach (var kv in bushOccupancy)
        {
            if (disabledBushes.Contains(kv.Key)) continue;
            foreach (string hider in kv.Value)
                if (activehiders.Contains(hider)) return true;
        }
        return false;
    }

    public void ReturnToGame() => SceneManager.LoadScene(ReturnSceneName);
}