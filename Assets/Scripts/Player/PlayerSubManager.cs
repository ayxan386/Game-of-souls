using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSubManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject boardPlayer;
    [SerializeField] private GameObject chaseGreenPlayer;

    public static List<PlayerSubManager> PlayerRoots;

    public Player BoardPlayer { get; private set; }

    public ChaseGreen_PlayerController ChaseGreenPlayer { get; private set; }

    public string PlayerId { get; private set; }

    private void Start()
    {
        if (PlayerRoots == null)
        {
            PlayerRoots = new List<PlayerSubManager>();
        }

        PlayerRoots.Add(this);
        BoardPlayer = boardPlayer.GetComponent<Player>();
        ChaseGreenPlayer = chaseGreenPlayer.GetComponent<ChaseGreen_PlayerController>();
        PlayerId = "Player " + (playerInput.playerIndex + 1);
    }

    public void LoadedChaseTheGreen()
    {
        playerInput.SwitchCurrentActionMap("ChaseTheGreen");
        boardPlayer.SetActive(false);
        chaseGreenPlayer.SetActive(true);
    }

    public void UnLoadedChaseTheGreen()
    {
        playerInput.SwitchCurrentActionMap("BoardControl");
        boardPlayer.SetActive(true);
        chaseGreenPlayer.SetActive(false);
    }
}