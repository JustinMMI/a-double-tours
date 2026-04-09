using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  

public class ReturnButton : MonoBehaviour
{
    public Button button;
        private const string ReturnSceneName = "GameScene";
    void Start()
    {
        
    }

    void Update()
    {
        button.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(ReturnSceneName));
    }
}
