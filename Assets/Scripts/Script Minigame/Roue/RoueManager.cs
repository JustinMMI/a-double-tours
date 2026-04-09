using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoueManager : MonoBehaviour
{
    private const string WinnerKey = "DuelWinner";
    private const string FromDuelKey = "FromDuel";

    [Header("Référence Roue")]
    [SerializeField] private RoueWheel roue;

    [Header("UI — Tour en cours")]
    [SerializeField] private TextMeshProUGUI textJoueurActuel;
    [SerializeField] private TextMeshProUGUI textInstructions;
    [SerializeField] private Button buttonStop;

    [Header("UI — Résultat d'un tour")]
    [SerializeField] private GameObject panelResultatTour;
    [SerializeField] private TextMeshProUGUI textResultatTour;
    [SerializeField] private Button buttonSuivant;

    [Header("UI — Résultat final")]
    [SerializeField] private GameObject panelResultatFinal;
    [SerializeField] private TextMeshProUGUI textResultatFinal;
    [SerializeField] private Button buttonRetour;

    private List<string> playerNames = new List<string>();
    private Dictionary<string, int> scores = new Dictionary<string, int>();
    private int currentPlayerIndex = 0;
    private bool gameOver = false;

    private void Start()
    {
        InitializePlayers();

        panelResultatTour.SetActive(false);
        panelResultatFinal.SetActive(false);

        buttonStop.onClick.AddListener(OnStopPressed);
        buttonSuivant.onClick.AddListener(OnNextPlayer);
        buttonRetour.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));

        ShowCurrentPlayer();
    }

    /// <summary>Charge les joueurs depuis PlayerPrefs ou utilise des noms par défaut.</summary>
    private void InitializePlayers()
    {
        int count = PlayerPrefs.GetInt("PlayerCount", 0);

        if (count < 2)
        {
            playerNames = new List<string> { "Alice", "Bob", "Charlie" };
            Debug.Log("[Roue] Mode debug — joueurs par défaut.");
        }
        else
        {
            for (int i = 0; i < count; i++)
                playerNames.Add(PlayerPrefs.GetString("Player_" + i, "Joueur " + (i + 1)));
        }

        foreach (string name in playerNames)
            scores[name] = 0;

        Debug.Log($"[Roue] {playerNames.Count} joueur(s) : {string.Join(", ", playerNames)}");
    }

    private void ShowCurrentPlayer()
    {
        string currentPlayer = playerNames[currentPlayerIndex];
        textJoueurActuel.text = currentPlayer;
        textInstructions.text = "Appuie sur STOP pour arrêter la roue !";
        buttonStop.interactable = true;
        roue.Restart();
    }

    private void OnStopPressed()
    {
        if (gameOver) return;

        buttonStop.interactable = false;

        RoueSection section = roue.Stop();
        string currentPlayer = playerNames[currentPlayerIndex];
        scores[currentPlayer] += section.points;

        string couleur = section.points >= 5 ? "#FFD700"
            : section.points >= 3 ? "#90EE90"
            : section.points > 0  ? "#FFFFFF"
            : "#FF6B6B";

        textResultatTour.text =
            $"<b>{currentPlayer}</b> tombe sur...\n" +
            $"<color={couleur}><size=120%>{section.nom}</size></color>\n" +
            $"+{section.points} point(s) !";

        bool isLastPlayer = currentPlayerIndex >= playerNames.Count - 1;
        buttonSuivant.GetComponentInChildren<TextMeshProUGUI>().text =
            isLastPlayer ? "Voir le résultat !" : "Joueur suivant →";

        panelResultatTour.SetActive(true);
    }

    private void OnNextPlayer()
    {
        panelResultatTour.SetActive(false);
        currentPlayerIndex++;

        if (currentPlayerIndex >= playerNames.Count)
            ShowFinalResult();
        else
            ShowCurrentPlayer();
    }

    private void ShowFinalResult()
    {
        gameOver = true;

        var tri = scores.OrderByDescending(x => x.Value).ToList();
        string winner = tri[0].Key;

        PlayerPrefs.SetString(WinnerKey, winner);
        PlayerPrefs.SetInt(FromDuelKey, 1);
        Debug.Log($"[Roue] Vainqueur : {winner}");

        string[] couleurs = { "#FFD700", "#C0C0C0", "#CD7F32", "#FF4500" };
        string recap = "<b>CLASSEMENT FINAL</b>\n\n";

        for (int i = 0; i < tri.Count; i++)
        {
            string c = i < couleurs.Length ? couleurs[i] : "#FFFFFF";
            recap += $"<color={c}>{i + 1}. {tri[i].Key}  —  {tri[i].Value} pt(s)</color>\n";
        }

        textResultatFinal.text = recap;
        panelResultatFinal.SetActive(true);
    }
}
