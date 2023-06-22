using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image health;
    [SerializeField] private TextMeshProUGUI soulCount;
    [SerializeField] private Animator animator;

    public void UpdateUI(Player player)
    {
        playerName.text = player.DisplayName;
        health.fillAmount = (1f * player.CurrentHealth) / player.MaxHealth;
        soulCount.text = player.SoulCount.ToString();
    }

    public void ToggleState(bool state)
    {
        animator.SetBool("active", state);
    }
}