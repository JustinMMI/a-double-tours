using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoAllRooms : MonoBehaviour
{
    public Button button;
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }
    void OnButtonClicked()
    {
        button.onClick.AddListener(() => SceneManager.LoadScene("allRooms"));
    }

    void Update()
    {
        
    }
}
