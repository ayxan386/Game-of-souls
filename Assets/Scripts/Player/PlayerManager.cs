using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    [SerializeField] private PathTile startingTile;
    [SerializeField] private Transform playerUIParent;
    [SerializeField] private Transform playerParent;
    [SerializeField] private GameObject boardScene;
    private int currentPlayer;

    public bool AllowPlayerSwitch { get; set; }

    public static PlayerManager Instance { get; private set; }

    public List<Player> Players => players;

    public Transform PlayerUIParent => playerUIParent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var player in players)
        {
            print("Player positioned");
        }

        DiceRotationManager.OnDiceRolled += OnDiceRolled;
        Player.OnPlayerPositionReached += OnPlayerPositionReached;
    }

    private void OnPlayerPositionReached(Player player)
    {
        StartCoroutine(WaitForEvent());
    }

    private IEnumerator WaitForEvent()
    {
        yield return new WaitUntil(() => AllowPlayerSwitch);
        players[currentPlayer].PlayerView.Priority = 5;
        players[currentPlayer].UpdateStateOfPlayer(false);
        currentPlayer = (currentPlayer + 1) % players.Count;
        ActivatePlayer();
        AllowPlayerSwitch = false;
    }

    private void ActivatePlayer()
    {
        players[currentPlayer].PlayerView.Priority = 15;
        players[currentPlayer].UpdateStateOfPlayer(true);
        DiceRotationManager.Instance.CanRoll = true;
    }

    private void OnDiceRolled(int diceRoll)
    {
        players[currentPlayer].MoveToTile(diceRoll);
        DiceRotationManager.Instance.CanRoll = false;
    }


    private void OnPlayerJoined(PlayerInput newPlayer)
    {
        var player = newPlayer.transform.GetComponent<Player>();
        newPlayer.transform.SetParent(playerParent);
        player.DisplayName = "Player " + newPlayer.playerIndex;

        players.Add(player);
        PathManager.Instance.StartPlayerAtPosition(player.DisplayName, startingTile);
        player.TeleportToPosition(startingTile.GetNextPoint().position);

        if (players.Count == 1)
        {
            ActivatePlayer();
        }
    }

    [ContextMenu("Load minigame")]
    public void LoadMiniGame()
    {
        boardScene.SetActive(false);
        SceneManager.LoadScene("Minigame1_ChaseGreen", LoadSceneMode.Additive);
    }

    public void MinigameFinished()
    {
        boardScene.SetActive(true);
    }
}