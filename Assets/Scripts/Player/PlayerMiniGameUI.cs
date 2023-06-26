using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMiniGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image coverImage;
    [SerializeField] private TextMeshProUGUI coverText;
    [SerializeField] private Image extraImage;

    public void UpdateUI(string playerName, int score, Color coverColor, string cover = "")
    {
        playerNameText.text = playerName;
        scoreText.text = score.ToString();
        coverImage.color = coverColor;
        coverText.alpha = string.IsNullOrEmpty(cover) ? 0 : 1;
        coverText.text = cover;
    }

    public void UpdateUI(string playerName, int score, Color coverColor, Sprite extraImageSprite)
    {
        playerNameText.text = playerName;
        scoreText.text = score.ToString();
        coverImage.color = coverColor;
        extraImage.sprite = extraImageSprite;
    }
}

public class PlayerRoundData
{
    public string playerName;
    public int place;
    public int score;
    public int finalAward;
    public bool isEliminated;
    public Color eliminationColor = Color.clear;
    public PlayerMiniGameUI gameUi;

    public void UpdateScore(int diff)
    {
        score += diff;
        UpdateUI();
    }

    public void UpdateUI()
    {
        gameUi.UpdateUI(playerName, score, eliminationColor);
    }

    public void UpdateSprite(Sprite sprite)
    {
        gameUi.UpdateUI(playerName, score, eliminationColor, sprite);
    }
}