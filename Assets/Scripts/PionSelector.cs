using UnityEngine;
using UnityEngine.UI;

public class PionSelector : MonoBehaviour
{
    public string pionName;
    public bool isSelected = false;

    private Image buttonImage;
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.color = defaultColor;
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        buttonImage.color = isSelected ? selectedColor : defaultColor;

        Debug.Log(pionName + " sélectionné : " + isSelected);
    }
}