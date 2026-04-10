using UnityEngine;
using UnityEngine.UI;

public class creditsbutton : MonoBehaviour
{
    public Button credits;
    public Canvas creditsCanvas;
    public Canvas MainMenuCanvas;

    void Start()
    {
        credits.onClick.AddListener(() =>
        {
            creditsCanvas.gameObject.SetActive(true);
            MainMenuCanvas.gameObject.SetActive(false);
        });
    }
}
