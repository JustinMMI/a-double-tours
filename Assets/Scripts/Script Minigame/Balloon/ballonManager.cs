using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BalloonManager : MonoBehaviour
{
    [Header("Player Name Labels")]
    [SerializeField] public TextMeshProUGUI player1Text;
    [SerializeField] public TextMeshProUGUI player2Text;

    [Header("Balloon References")]
    [SerializeField] public ballon balloon1;
    [SerializeField] public ballon balloon2;
    public Button QuitterPanelButton;
    public GameObject Panel;

    public const string FallbackPlayer1 = "Joueur 1";
    public const string FallbackPlayer2 = "Joueur 2";

    public void Start()
    {
        if (QuitterPanelButton != null)
            QuitterPanelButton.onClick.AddListener(() =>
            {
                Panel.SetActive(false);
            });
    }
private void Awake()
{
    int playerCount = PlayerPrefs.GetInt("DuelPlayerCount", 0);

    player1Text.text = playerCount > 0
        ? PlayerPrefs.GetString("DuelPlayer_0", FallbackPlayer1)
        : FallbackPlayer1;

    player2Text.text = playerCount > 1
        ? PlayerPrefs.GetString("DuelPlayer_1", FallbackPlayer2)
        : FallbackPlayer2;

    if (balloon1 != null) balloon1.ownerPlayerName = player1Text.text;
    if (balloon2 != null) balloon2.ownerPlayerName = player2Text.text;
}


    public void NotifyOpponent(ballon winner)
    {
        if (balloon1 != null && balloon1 != winner)
            balloon1.OnOpponentWon();

        if (balloon2 != null && balloon2 != winner)
            balloon2.OnOpponentWon();
    }

        public void ResolveBySize()
        {
            ballon[] balloons = FindObjectsByType<ballon>();

            ballon biggest = null;
            foreach (ballon b in balloons)
            {
                if (biggest == null || b.GetSize() > biggest.GetSize())
                    biggest = b;
            }

            if (biggest == null) return;

            string winnerName = biggest.ownerPlayerName == player1Text.text
                ? player1Text.text
                : player2Text.text;

            foreach (ballon b in balloons)
                b.DeclareWinner(winnerName);
        }
}
