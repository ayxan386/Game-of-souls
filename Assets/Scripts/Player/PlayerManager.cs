using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    [SerializeField] private PathTile startingTile;
    [SerializeField] private Transform playerUIParent;
    [SerializeField] private PlayerInputManager playerInputManager;

    public static PlayerManager Instance { get; private set; }

    public Transform PlayerUIParent => playerUIParent;

    private int currentPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DiceRotationManager.OnDiceRolled += OnDiceRolled;
    }

    private void OnDisable()
    {
        DiceRotationManager.OnDiceRolled -= OnDiceRolled;
    }

    private IEnumerator WaitThenSetUp(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(0.2f);
        var playerSubManager = playerInput.GetComponent<PlayerSubManager>();
        var player = playerSubManager.BoardPlayer;
        player.DisplayName = playerSubManager.PlayerId;

        players.Add(player);
        PathManager.Instance.StartPlayerAtPosition(player.DisplayName, startingTile);
        player.TeleportToPosition(startingTile.GetNextPoint().position);

        if (players.Count == 1)
        {
            ActivatePlayer();
        }

        player.UpdateSoulCount(0);
    }

    public void EndPlayerTurn()
    {
        players[currentPlayer].PlayerView.Priority = 5;
        players[currentPlayer].UpdateStateOfPlayer(false);
        print("Ended player turn");
        currentPlayer = (currentPlayer + 1) % players.Count;
        print("Current player " + currentPlayer);
        ActivatePlayer();
    }

    private void ActivatePlayer()
    {
        players[currentPlayer].PlayerView.Priority = 15;
        players[currentPlayer].UpdateStateOfPlayer(true);
        DiceRotationManager.Instance.CanRoll = true;
    }

    private void OnDiceRolled(int diceRoll)
    {
        playerInputManager.DisableJoining();
        players[currentPlayer].MoveToTile(diceRoll);
        DiceRotationManager.Instance.CanRoll = false;
    }

    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        StartCoroutine(WaitThenSetUp(newPlayer));
    }

    public void AwardPlayerWithSouls(string playerName, int soulCount)
    {
        players.Find(player => player.DisplayName == playerName).UpdateSoulCount(soulCount);
    }
}