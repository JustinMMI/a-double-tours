using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SendToAllRooms : MonoBehaviour
{
    public Button Main;
    public Button GameScene;
    public Button DuelMenu;
    public Button Gonflement;
    public Button Mashing;
    public Button CacheCache;
    public Button ChiffreDeBob;
    public Button ChronoGame;
    public Button Miner;
    public Button Miroir;
    public Button Roue;


    void Start()
    {
        Main.onClick.AddListener(() => SceneManager.LoadScene("Main"));
        GameScene.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
        DuelMenu.onClick.AddListener(() => SceneManager.LoadScene("DuelMenu"));
        Gonflement.onClick.AddListener(() => SceneManager.LoadScene("Gonflement ballon"));
        Mashing.onClick.AddListener(() => SceneManager.LoadScene("Mashing"));
        CacheCache.onClick.AddListener(() => SceneManager.LoadScene("CacheCache"));
        ChiffreDeBob.onClick.AddListener(() => SceneManager.LoadScene("Chiffre De Bob"));
        ChronoGame.onClick.AddListener(() => SceneManager.LoadScene("ChronoGame"));
        Miner.onClick.AddListener(() => SceneManager.LoadScene("Miner"));
        Miroir.onClick.AddListener(() => SceneManager.LoadScene("MiroirGames"));
        Roue.onClick.AddListener(() => SceneManager.LoadScene("Roue"));

    }

    void Update()
    {
        
    }
}
