using TMPro;
using UnityEngine;

public class PlayerAwardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI soulCountText;

    public void UpdateUI(int place, string playerName, int soulCount)
    {
        var postfix = place switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };

        placeText.text = place + postfix;
        playerNameText.text = playerName;
        soulCountText.text = soulCount.ToString();
    }
}