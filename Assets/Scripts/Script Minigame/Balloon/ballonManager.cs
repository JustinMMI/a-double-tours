// BalloonManager.cs
using UnityEngine;
using TMPro;

public class BalloonManager : MonoBehaviour
{
    [Header("Player Name Labels")]
    [SerializeField] public TextMeshProUGUI player1Text;
    [SerializeField] public TextMeshProUGUI player2Text;

    public const string FallbackPlayer1 = "Joueur 1";
    public const string FallbackPlayer2 = "Joueur 2";

    public void Start()
    {
        // Fix 6: null-check before use
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
    }
}