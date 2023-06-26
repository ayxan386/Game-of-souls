using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinedIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image playerColor;

    public void Display(string name, Color color)
    {
        playerName.text = name;
        playerColor.color = color;
    }
}