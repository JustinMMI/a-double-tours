// BalloonManager.cs
using UnityEngine;
using TMPro;

public class BalloonManager : MonoBehaviour
{
    [Header("Player Name Labels")]
    [SerializeField] public TextMeshProUGUI player1Text;
    [SerializeField] public TextMeshProUGUI player2Text;

    [Header("Balloon References")]
    [SerializeField] public ballon balloon1;
    [SerializeField] public ballon balloon2;

    public const string FallbackPlayer1 = "Joueur 1";
    public const string FallbackPlayer2 = "Joueur 2";

private void Awake()
{
    if (player1Text == null || player2Text == null)
    {
        Debug.LogError("[BalloonManager] Player text references are not assigned in the Inspector.");
        return;
    }

    int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);

    player1Text.text = playerCount > 0
        ? PlayerPrefs.GetString("Player_0", FallbackPlayer1)
        : FallbackPlayer1;

    player2Text.text = playerCount > 1
        ? PlayerPrefs.GetString("Player_1", FallbackPlayer2)
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
